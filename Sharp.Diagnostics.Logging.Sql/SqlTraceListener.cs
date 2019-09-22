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
using System.Diagnostics;
using Newtonsoft.Json;

#if NETFRAMEWORK
using System.Configuration;
#endif

#if NETSTANDARD
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
#endif

namespace Sharp.Diagnostics.Logging.Sql
{
    /// <summary>
    ///   A trace listener that writes trace events to a log database hosted in
    ///   SQL Server or Azure SQL Database.
    /// </summary>
    public class SqlTraceListener : TraceListener
    {
        private static readonly TraceSource TraceSource
            = new TraceSource("SqlTraceListener");

#if NETFRAMEWORK
        /// <summary>
        ///   Initializes a new <see cref="SqlTraceListener"/> instance with
        ///   the specified connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        ///   The name of the connection string for the log database.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="connectionStringName"/> is <c>null</c>.
        /// </exception>
        public SqlTraceListener(string connectionStringName)
        {
            const string
                KeyPrefix          = "SqlTraceListener.",
                ApplicationNameKey = KeyPrefix + nameof(LogEntry.Application),
                EnvironmentNameKey = KeyPrefix + nameof(LogEntry.Environment),
                ComponentNameKey   = KeyPrefix + nameof(LogEntry.Component);

            if (connectionStringName is null)
                throw new ArgumentNullException(nameof(connectionStringName));

            // Read connection string
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName]
                ?? throw new ConfigurationErrorsException(string.Format(
                    "A connection string named '{0}' was not found.",
                    connectionStringName
                ));

            // Read app settings
            var section     = ConfigurationManager.AppSettings;
            ApplicationName = section[ApplicationNameKey] ?? GetDefaultApplicationName();
            EnvironmentName = section[EnvironmentNameKey] ?? "Default";
            ComponentName   = section[ComponentNameKey  ] ?? "Default";

            Writer = SqlLogWriter.WithConnectionString(connectionString.ConnectionString);
        }
#endif
#if NETSTANDARD
        /// <summary>
        ///   Initializes a new <see cref="SqlTraceListener"/> instance with
        ///   the specified connection string name and configuration.
        /// </summary>
        /// <param name="connectionStringName">
        ///   The name of the connection string for the log database.
        /// </param>
        /// <param name="configuration">
        ///   The application configuration to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="connectionStringName"/> is <c>null</c>, or
        ///   <paramref name="configuration"/>        is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="connectionStringName"/> is empty.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///   A connection string with name
        ///   <paramref name="connectionStringName"/> was not found.
        /// </exception>
        public SqlTraceListener(string connectionStringName, IConfiguration configuration)
        {
            if (connectionStringName is null)
                throw new ArgumentNullException(nameof(connectionStringName));
            if (connectionStringName.Length == 0)
                throw new ArgumentException("Argument cannot be empty.", nameof(connectionStringName));
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            var connectionString = configuration.GetConnectionString(connectionStringName)
                ?? throw new KeyNotFoundException(string.Format(
                    "The configuration value with key 'ConnectionStrings:{0}' was not found.",
                    connectionStringName
                ));

            var section     = configuration.GetSection(nameof(SqlTraceListener));
            ApplicationName = section[nameof(LogEntry.Application)] ?? GetDefaultApplicationName();
            EnvironmentName = section[nameof(LogEntry.Environment)] ?? "Default";
            ComponentName   = section[nameof(LogEntry.Component  )] ?? "Default";

            Writer = SqlLogWriter.WithConnectionString(connectionString);
        }
#endif
        // For testing
        internal SqlTraceListener(ILogWriter writer)
        {
            Writer = writer;
        }

        /// <summary>
        ///   Gets the log writer used by the trace listener.
        /// </summary>
        public ILogWriter Writer { get; }

        /// <summary>
        ///   Gets or sets the name of the application.  This value populates
        ///   the <see cref="LogEntry.Application"/> property.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///   Gets or sets the name of the application.  This value populates
        ///   the <see cref="LogEntry.Environment"/> property.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        ///   Gets or sets the name of the application.  This value populates
        ///   the <see cref="LogEntry.Component"/> property.
        /// </summary>
        public string ComponentName { get; set; }

        /// <inheritdoc/>
        public override void Flush()
        {
            Writer.Flush();
        }

        /// <inheritdoc/>
        public override void Write(string message)
        {
            TraceEvent(new TraceEventCache(), "", TraceEventType.Information, 0, message);
        }

        /// <inheritdoc/>
        public override void WriteLine(string message)
        {
            TraceEvent(new TraceEventCache(), "", TraceEventType.Information, 0, message);
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id)
        {
            TraceEvent(e, source, type, id, "");
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id, string message)
        {
            if (!ShouldTrace(e, source, type, id, message))
                return;

            TraceEventCore(e, source, type, id, message);
        }

        /// <inheritdoc/>
        public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id, string format, params object[] args)
        {
            if (!ShouldTrace(e, source, type, id, format, args))
                return;

            TraceEventCore(e, source, type, id, args == null
                ? format
                : string.Format(format, args)
            );
        }

        /// <inheritdoc/>
        public override void TraceData(TraceEventCache e, string source, TraceEventType type, int id, object obj)
        {
            if (!ShouldTrace(e, source, type, id, obj: obj))
                return;

            var entry = CreateEntry(e, source, type, id);

            entry.Data.Add(CreateData(obj));

            Writer.Enqueue(entry);
        }

        /// <inheritdoc/>
        public override void TraceData(TraceEventCache e, string source, TraceEventType type, int id, params object[] objs)
        {
            if (!ShouldTrace(e, source, type, id, objs: objs))
                return;

            var entry = CreateEntry(e, source, type, id);

            foreach (var obj in objs)
                entry.Data.Add(CreateData(obj));

            Writer.Enqueue(entry);
        }

        /// <inheritdoc/>
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

            Writer.Enqueue(entry);
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

        /// <inhertidoc/>
        public override void Close()
        {
            Dispose();
        }

        /// <inhertidoc/>
        protected override void Dispose(bool managed)
        {
            base.Dispose(managed);

            Writer.Dispose();
        }
    }
}
