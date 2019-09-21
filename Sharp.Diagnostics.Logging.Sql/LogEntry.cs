using System;
using System.Diagnostics;

namespace Sharp.Diagnostics.Logging.Sql
{
    internal class LogEntry
    {
        public int            Id   { get; set; }
        public DateTime       Date { get; set; }
        public TraceEventType Type { get; set; }

        public string Application { get; set; }
        public string Environment { get; set; }
        public string Component   { get; set; }
        public string Machine     { get; set; }
        public string Source      { get; set; }
        public int?   ProcessId   { get; set; }
        public int?   ThreadId    { get; set; }
        public Guid?  ActivityId  { get; set; }
        public int    MessageId   { get; set; }
        public string Message     { get; set; }

        public static readonly ObjectDataMap<LogEntry>
            Map = new ObjectDataMap<LogEntry>
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
