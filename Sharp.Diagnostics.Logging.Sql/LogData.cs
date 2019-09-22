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

using Sharp.Data;

namespace Sharp.Diagnostics.Logging.Sql
{
    /// <summary>
    ///   Arbitrary textual data associated with a <see cref="LogEntry"/>
    ///   object.
    /// </summary>
    public class LogData
    {
        // Association with LogEvent
        internal int EntryId { get; set; }

        /// <summary>
        ///   Gets or sets a value that describes the format of the
        ///   <see cref="Data"/> property value.
        /// </summary>
        public LogDataType Type { get; set; }

        /// <summary>
        ///   Arbitrary textual data in the format described by the
        ///   <see cref="Type"/> property value.
        /// </summary>
        public string Data { get; set; }

        internal static ObjectDataMap<LogData> Map { get; }
            = new ObjectDataMap<LogData>
            (b => b
                .Field("EntryId",  "int",           d => d.EntryId)
                .Field("TypeCode", "char(1)",       d => GetTypeCode(d.Type))
                .Field("Data",     "nvarchar(max)", d => d.Data)
            );

        private static string GetTypeCode(LogDataType type)
        {
            switch (type)
            {
                default:
                case LogDataType.Text:                  return "T";
                case LogDataType.Json:                  return "J";
                case LogDataType.CallStack:             return "C";
                case LogDataType.LogicalOperationStack: return "L";
            }
        }
    }
}
