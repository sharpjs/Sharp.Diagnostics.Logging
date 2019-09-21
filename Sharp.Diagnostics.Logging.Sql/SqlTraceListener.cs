/*
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
using System.Collections;
using System.Collections.Concurrent;
//using System.Configuration;
using System.Data;
//using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

// Temporarily
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sharp.Diagnostics.Logging.Sql
{
    public class SqlTraceListener : TraceListener
    {
        private const int
            MaxMessageLength      = 1024,
            CommandTimeoutSeconds = 3 * 60; // 3 minutes

        private const string
            ApplicationNameKey = "SqlTraceListener.Application",
            EnvironmentNameKey = "SqlTraceListener.Environment",
            ComponentNameKey   = "SqlTraceListener.Component";

        private static readonly TraceSource TraceSource
            = new TraceSource("SqlTraceListener");

        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
        public string ComponentName   { get; set; }

        public TimeSpan AutoflushWait      { get; set; } = TimeSpan.FromSeconds ( 5);
        public TimeSpan CloseWait          { get; set; } = TimeSpan.FromSeconds (10);
        public TimeSpan RetryWaitIncrement { get; set; } = TimeSpan.FromMinutes ( 5);
        public TimeSpan RetryWaitMax       { get; set; } = TimeSpan.FromHours   ( 1);

        private readonly string                    _connectionString;
        private readonly ConcurrentQueue<LogEntry> _queue;
        private readonly AutoResetEvent            _flushEvent;
        private readonly Thread                    _flushThread;
        private readonly SqlCommand                _command;

        // Used by flush thread
        private SqlConnection _connection;
        private DateTime      _flushTime;
        private bool          _exiting;

        public SqlTraceListener(string connectionStringName)
        {
            if (connectionStringName == null)
                throw new ArgumentNullException(nameof(connectionStringName));

            //// Read app settings
            //var config      = ConfigurationManager.AppSettings;
            //ApplicationName = config[ApplicationNameKey] ?? GetDefaultApplicationName();
            //EnvironmentName = config[EnvironmentNameKey] ?? "Default";
            //ComponentName   = config[ComponentNameKey  ] ?? "Default";

            // Read connection string
            _connectionString = GetConnectionString(connectionStringName);

            // Initialize event queue
            _queue = new ConcurrentQueue<LogEntry>();

            // Prepare flush command
            _command = new SqlCommand()
            {
                CommandType    = CommandType.StoredProcedure,
                CommandText    = "WriteLog",
                CommandTimeout = CommandTimeoutSeconds
            };
            var parameter = _command.Parameters.Add("@EntryRows", SqlDbType.Structured);
            parameter.TypeName = "dbo.LogEntryRow";

            // Start flush thread
            _flushEvent  = new AutoResetEvent(initialState: false);
            _flushThread = new Thread(FlushThreadMain)
            {
                Name         = "SqlTraceListener.Flush",
                Priority     = ThreadPriority.AboveNormal,
                IsBackground = true, // don't prevent app from exiting
            };
            _flushThread.Start();
        }

        private static string GetConnectionString(string name)
        {
            //var item = ConfigurationManager.ConnectionStrings[name];
            //if (item != null)
            //    return item.ConnectionString;

            throw new /*ConfigurationErrorsException*/ Exception(string.Format(
                "A connection string named '{0}' was not found.",
                name
            ));
        }

        public override void Close()
        {
            Dispose();
        }

        protected override void Dispose(bool managed)
        {
            if (managed && !_exiting)
            {
                // Tell flusher thread to flush and exit
                _exiting = true;
                _flushEvent.Set();

                // Give flusher thread a fair chance to flush
                if (!_flushThread.Join(CloseWait))
                    _flushThread.Abort();

                // Dispose managed objects
                _flushEvent.Dispose();
                _command.Dispose();
            }

            base.Dispose(managed);
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
            // Avoid flushing an empty queue
            var queue = _queue;
            if (queue.IsEmpty)
                return;

            // Take a snapshot of the pending events at this time
            var events = queue.ToArray();
            var count  = events.Length;

            // Prepare queue snapshot for transmission
            RenumberEvents(events);

            // Ensure good connection to the server
            var connection = _connection;
            if (connection != null && connection.State != ConnectionState.Open)
            {
                connection.Dispose();
                connection = null;
            }
            if (connection == null)
            {
                connection          = new SqlConnection(_connectionString); //.Logged();
                //DisposalTracker<DbConnection>.Instance.Track(connection);
                _connection         = connection;
                _command.Connection = connection;
                connection.Open();
            }

            var data = events
                .Where      (e => e.HasData)
                .SelectMany (e => e.Data);

            // Transmit the events to the server
            _command.Parameters[0].Value = new ObjectDataReader<LogEntry>(events, LogEntry.Map);
            _command.Parameters[1].Value = new ObjectDataReader<LogData> (data,   LogData .Map);
            _command.ExecuteNonQuery();

            // Remove the events from the queue
            for (; count > 0; count--)
                queue.TryDequeue(out LogEntry entry);
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

        public override void Flush()
        {
            _flushEvent.Set();
        }

        public override void Write(string message)
        {
            TraceEvent(new TraceEventCache(), "", TraceEventType.Information, 0, message);
        }

        public override void WriteLine(string message)
        {
            TraceEvent(new TraceEventCache(), "", TraceEventType.Information, 0, message);
        }

        public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id)
        {
            TraceEvent(e, source, type, id, "");
        }

        public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id, string message)
        {
            if (!ShouldTrace(e, source, type, id, message))
                return;

            TraceEventCore(e, source, type, id, message);
        }

        public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id, string format, params object[] args)
        {
            if (!ShouldTrace(e, source, type, id, format, args))
                return;

            TraceEventCore(e, source, type, id, args == null
                ? format
                : string.Format(format, args)
            );
        }

        public override void TraceData(TraceEventCache e, string source, TraceEventType type, int id, object obj)
        {
            if (!ShouldTrace(e, source, type, id, obj: obj))
                return;

            var entry = CreateEntry(e, source, type, id);
            entry.Data.Add(CreateData(obj));
            _queue.Enqueue(entry);
        }

        public override void TraceData(TraceEventCache e, string source, TraceEventType type, int id, params object[] objs)
        {
            if (!ShouldTrace(e, source, type, id, objs: objs))
                return;

            var entry = CreateEntry(e, source, type, id);
            foreach (var obj in objs)
                entry.Data.Add(CreateData(obj));
            _queue.Enqueue(entry);
        }

        public override void TraceTransfer(TraceEventCache e, string source, int id, string message, Guid relatedActivityId)
        {
            TraceEvent(e, source, TraceEventType.Transfer, id, "{0} {related:{1}}", message, relatedActivityId.ToString());
        }

        private bool ShouldTrace(TraceEventCache e, string source, TraceEventType type, int id,
            string message = null, object[] args = null, object obj = null, object[] objs = null)
        {
            var filter = Filter;
            return filter == null
                || filter.ShouldTrace(e, source, type, id, message, args, obj, objs);
        }

        private void TraceEventCore(TraceEventCache e, string source, TraceEventType type, int id, string message)
        {
            var entry = CreateEntry(e, source, type, id);
            entry.Message = Truncate(message, MaxMessageLength);
            _queue.Enqueue(entry);
        }

        private LogEntry CreateEntry(TraceEventCache e, string source, TraceEventType type, int id)
        {
            int.TryParse(e.ThreadId, out int threadId);

            var activityId = Trace.CorrelationManager.ActivityId as Guid?;
            if (activityId == Guid.Empty)
                activityId = null;

            return new LogEntry
            {
                Date        = e.DateTime,
                Type        = type,
                Application = ApplicationName,
                Environment = EnvironmentName,
                Component   = ComponentName,
                Machine     = Environment.MachineName,
                Source      = source,
                ProcessId   = e.ProcessId,
                ThreadId    = threadId,
                ActivityId  = activityId,
                MessageId   = id
            };
        }

        private static LogData CreateData(object obj)
        {
            var data = new LogData();

            if (obj is string s)
            {
                data.Type = LogDataType.Text;
                data.Data = s;
            }
            else if (obj is Exception e)
            {
                data.Type = LogDataType.CallStack;
                data.Data = e.StackTrace;
            }
            else if (obj == Trace.CorrelationManager.LogicalOperationStack)
            {
                data.Type = LogDataType.LogicalOperationStack;
                data.Data = string.Join(Environment.NewLine, Trace.CorrelationManager.LogicalOperationStack);
            }
            else
            {
                data.Type = LogDataType.Json;
                data.Data = JsonConvert.SerializeObject(obj);
            }

            return data;
        }

        private static void RenumberEvents(LogEntry[] events)
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

        private static string GetDefaultApplicationName()
            => Process.GetCurrentProcess().ProcessName;

        private static string Truncate(string s, int length)
            => s.Length <= length ? s : s.Substring(0, length);
    }
}
