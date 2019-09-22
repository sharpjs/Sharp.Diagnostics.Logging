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
using System.Collections.Generic;
using System.Diagnostics;
using Sharp.Data;

namespace Sharp.Diagnostics.Logging.Sql
{
    /// <summary>
    ///   An entry in a log.
    /// </summary>
    public class LogEntry
    {
        internal int Id { get; set; }

        /// <summary>
        ///   Gets or sets the date of the log entry.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///   Gets or sets the type of the log entry.
        /// </summary>
        public TraceEventType Type { get; set; }

        /// <summary>
        ///   Gets or sets the name of the application that produced the log
        ///   entry.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        ///   Gets or sets the name of the application environment that
        ///   produced the log entry.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        ///   Gets or sets the name of the application component that produced
        ///   the log entry.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        ///   Gets or sets the name of the machine that produced the log entry.
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        ///   Gets or sets an arbitrary source name.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///   Gets or sets the id of the operating system process that produced
        ///   the log entry.
        /// </summary>
        public int? ProcessId { get; set; }

        /// <summary>
        ///   Gets or sets the id of the managed thread that produced the log
        ///   entry.
        /// </summary>
        public int? ThreadId { get; set; }

        /// <summary>
        ///   Gets or sets the id of the logical activity that produced the log
        ///   entry.
        /// </summary>
        public Guid? ActivityId { get; set; }

        /// <summary>
        ///   Gets or sets the id of the message of the log entry.
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        ///   Gets or sets the message of the log entry.
        ///   Values are limited to 1024 characters.
        /// </summary>
        public string Message { get; set; }

        private IList<LogData> _data;

        /// <summary>
        ///   Gets a value indicating whether any arbitrary textual data is
        ///   associated with the log entry via the <see cref="Data"/> property.
        /// </summary>
        public bool HasData
            => _data?.Count > 0;

        /// <summary>
        ///   Gets a collection of arbitrary textual data associated with the
        ///   log entry.
        /// </summary>
        public IList<LogData> Data
            => _data ?? (_data = new List<LogData>());

        internal static ObjectDataMap<LogEntry> Map { get; }
            = new ObjectDataMap<LogEntry>
            (b => b
                .Field("Id",          "int",              e => e.Id)
                .Field("Date",        "datetime2(3)",     e => e.Date)
                .Field("TypeCode",    "char(1)",          e => GetTypeCode(e.Type))
                .Field("Application", "varchar(32)",      e => e.Application)
                .Field("Environment", "varchar(32)",      e => e.Environment)
                .Field("Component",   "varchar(32)",      e => e.Component)
                .Field("Machine",     "varchar(128)",     e => e.Machine)
                .Field("Source",      "varchar(128)",     e => e.Source)
                .Field("ProcessId",   "int",              e => e.ProcessId)
                .Field("ThreadId",    "int",              e => e.ThreadId)
                .Field("ActivityId",  "uniqueidentifier", e => e.ActivityId)
                .Field("MessageId",   "int",              e => e.MessageId)
                .Field("Message",     "varchar(1024)",    e => e.Message)
            );

        private static string GetTypeCode(TraceEventType type)
        {
            switch (type)
            {
                case TraceEventType.Critical:    return "C";
                case TraceEventType.Error:       return "E";
                case TraceEventType.Warning:     return "W";
                case TraceEventType.Information: return "I";
                case TraceEventType.Verbose:     return "V";
                case TraceEventType.Start:       return "<";
                case TraceEventType.Stop:        return ">";
                case TraceEventType.Suspend:     return "-";
                case TraceEventType.Resume:      return "+";
                case TraceEventType.Transfer:    return "=";
                default: /* treat as Verbose */  return "V";
            }
        }
    }
}
