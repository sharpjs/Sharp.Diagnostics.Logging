﻿/*
    Copyright (C) 2019 Jeffrey Sharp

    Permission to use, copy, modify, and distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using SD = Sharp.Disposable;

// Temporarily
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sharp.Diagnostics.Logging.Sql
{
    /// <summary>
    ///   A background worker that periodically flushes a queue of log entries
    ///   to a database hosted in SQL Server or Azure SQL Database.
    /// </summary>
    internal class SqlLogWriter : SD.Disposable
    {
        private const int
            MaxMessageLength      = 1024,
            CommandTimeoutSeconds = 3 * 60; // 3 minutes

        // Set by constructor
        private readonly ConcurrentQueue<LogEntry> _queue;
        private readonly AutoResetEvent            _flushEvent;
        private readonly SqlCommand                _flushCommand;
        private readonly Thread                    _flushThread;
        private readonly string                    _connectionString;

        // Set by flush thread
        private SqlConnection _connection;
        private DateTime      _flushTime;
        private bool          _exiting;

        // Configurables
        public TimeSpan AutoflushWait      { get; set; } = TimeSpan.FromSeconds ( 5);
        public TimeSpan CloseWait          { get; set; } = TimeSpan.FromSeconds (10);
        public TimeSpan RetryWaitIncrement { get; set; } = TimeSpan.FromMinutes ( 5);
        public TimeSpan RetryWaitMax       { get; set; } = TimeSpan.FromHours   ( 1);

        /// <summary>
        ///   Initializes a new <see cref="SqlLogWriter"/> instance with the
        ///   specified connection string.
        /// </summary>
        /// <param name="connectionString">
        ///   The connection string for the log database.
        /// </param>
        public SqlLogWriter(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));

            _queue        = new ConcurrentQueue<LogEntry>();
            _flushEvent   = new AutoResetEvent(initialState: false);
            _flushCommand = CreateFlushCommand();
            _flushThread  = CreateFlushThread();

            _flushThread.Start();
        }

        private SqlCommand CreateFlushCommand()
        {
            return new SqlCommand()
            {
                CommandType    = CommandType.StoredProcedure,
                CommandText    = "WriteLog",
                CommandTimeout = CommandTimeoutSeconds,
                Parameters     =
                {
                    new SqlParameter
                    {
                        ParameterName = "@EntryRows",
                        SqlDbType     = SqlDbType.Structured,
                        TypeName      = "dbo.LogEntryRow"
                    },
                    new SqlParameter
                    {
                        ParameterName = "@DataRows",
                        SqlDbType     = SqlDbType.Structured,
                        TypeName      = "dbo.LogDataRow"
                    }
                }
            };
        }

        private Thread CreateFlushThread()
        {
            return new Thread(FlushThreadMain)
            {
                Name             = nameof(SqlLogWriter),
                Priority         = ThreadPriority.AboveNormal,
                IsBackground     = false, // prevents app from exiting
                CurrentCulture   = CultureInfo.InvariantCulture,
                CurrentUICulture = CultureInfo.InvariantCulture
            };
        }

        private void FlushThreadMain()
        {
            ScheduleAutoflush();

            for (var retries = 0;;)
            {
                try
                {
                    if (_exiting)
                        return;

                    WaitBeforeFlush();
                    FlushCore();

                    retries = 0;
                }
                catch (Exception e)
                {
                    OnException(e);
                    WaitBeforeRetry(retries);

                    if (retries < int.MaxValue)
                        retries++;
                }
            }
        }

        private void ScheduleAutoflush()
        {
            // Compute time of next autoflush
            _flushTime = DateTime.UtcNow + AutoflushWait;
        }

        private void WaitBeforeFlush()
        {
            // Compute time remaining until autoflush
            var duration = _flushTime - DateTime.UtcNow;

            // Wait until autoflush time, or until Flush() called
            if (duration > TimeSpan.Zero)
                _flushEvent.WaitOne(duration);

            // Compute time of next autoflush
            ScheduleAutoflush();
        }

        private void FlushCore()
        {
            if (!TryGetQueueSnapshot(out var entries))
                return;

            Prepare(entries);

            EnsureConnection();
            ExecuteFlushCommand(entries);

            Dequeue(entries);
        }

        private bool TryGetQueueSnapshot(out LogEntry[] entries)
        {
            var queue = _queue;

            if (queue.IsEmpty)
            {
                entries = null;
                return false;
            }
            else
            {
                entries = queue.ToArray();
                return true;
            }
        }

        private static void Prepare(LogEntry[] events)
        {
            var id = 0;

            foreach (var e in events)
            {
                e.Id = id;

                if (e.HasData)
                    foreach (var d in e.Data)
                        d.EntryId = id;

                id++;
            }
        }

        private void EnsureConnection()
        {
            var connection = _connection;

            if (connection != null && connection.State != ConnectionState.Open)
            {
                // Connection is broken; throw it away
                connection.Dispose();
                connection = null;
            }

            if (connection == null)
            {
                connection
                    = _connection
                    = _flushCommand.Connection
                    = new SqlConnection(_connectionString); //.Logged();

                connection.Open();
            }
        }

        private void ExecuteFlushCommand(LogEntry[] entries)
        {
            var command = _flushCommand;

            var data = entries
                .Where(e => e.HasData)
                .SelectMany(e => e.Data);

            using (var entryRows = new ObjectDataReader<LogEntry>(entries, LogEntry.Map))
            using (var dataRows  = new ObjectDataReader<LogData >(data,    LogData .Map))
            {
                _flushCommand.Parameters[0].Value = entryRows;
                _flushCommand.Parameters[1].Value = dataRows;
                _flushCommand.ExecuteNonQuery();
            }
        }

        private void Dequeue(LogEntry[] entries)
        {
            var queue = _queue;

            for (var count = entries.Length; count > 0; count--)
                queue.TryDequeue(out _);
        }

        private void OnException(Exception e)
        {
            //TraceSource.TraceError(e);
        }

        private void WaitBeforeRetry(int retries)
        {
            // Adaptive delay: nothing for the first retry, then increasing by
            // regular increments for each successive retry, up to some maximum.
            // Default: wait 5 minutes longer for each retry, up to 1 hour max.

            var incrementsMax = RetryWaitMax.Ticks / RetryWaitIncrement.Ticks;
            var increments    = Math.Min((long) retries, incrementsMax);
            var duration      = new TimeSpan(increments * RetryWaitIncrement.Ticks);

            //TraceSource.TraceWarning("Flush failed; retrying after {0:c}.", duration);

            Thread.Sleep(duration);
        }

        public void Flush()
        {
            _flushEvent.Set();
        }

        public void Enqueue(LogEntry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));

            entry.Message = Truncate(entry.Message, MaxMessageLength);

            _queue.Enqueue(entry);
        }

        private static string Truncate(string s, int length)
            => s.Length <= length ? s : s.Substring(0, length);

        /// <inheritdoc/>
        protected override bool Dispose(bool managed = true)
        {
            if (!base.Dispose(managed))
                return false;

            if (!managed)
                return true;

            if (_exiting)
                return false;

            // Tell flusher thread to flush and exit
            _exiting = true;
            _flushEvent.Set();

            // Give flusher thread a fair chance to flush
            if (!_flushThread.Join(CloseWait))
                _flushThread.Abort();

            // Dispose managed objects
            _flushEvent.Dispose();
            _flushCommand.Dispose();

            return true;
        }
    }
}