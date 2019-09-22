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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Sharp.Data
{
    internal class ObjectDataReader<T> : DbDataReader
    {
        private readonly ObjectDataMap<T> _map;
        private readonly IEnumerator<T>   _rows;
        private readonly bool             _hasRows;

        public ObjectDataReader(IEnumerable<T> source, ObjectDataMap<T> map)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            _map     = map;
            _hasRows = source.Any();
            _rows    = source.GetEnumerator();
        }

        protected override void Dispose(bool managed)
        {
            if (managed)
                _rows.Dispose();

            base.Dispose(managed);
        }

        public override int  FieldCount      => _map.Count;
        public override bool HasRows         => _hasRows;
        public override bool IsClosed        => false;  // Cannot be closed
        public override int  Depth           =>  0;     // Cannot be nested
        public override int  RecordsAffected => -1;     // Expected value for SELECT queries

        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => GetValue(GetOrdinal(name));

        public override bool NextResult()
            { while (_rows.MoveNext()); return false; }

        public override bool Read()
            => _rows.MoveNext();

        public override string GetName(int ordinal)
            => _map[ordinal].Name;
        
        public override Type GetFieldType(int ordinal)
            => _map[ordinal].NetType;

        public override string GetDataTypeName(int ordinal)
            => _map[ordinal].DbType;

        public override int GetOrdinal(string name)
            => _map.GetOrdinal(name);

        public override bool IsDBNull(int ordinal)
            => GetValue(ordinal) == null;

        public override object GetValue(int ordinal)
            => _map[ordinal].GetValue(_rows.Current);

        public TValue GetValueAs<TValue>(int ordinal)
            => _map[ordinal].GetValueAs<TValue>(_rows.Current);

        public override string GetString(int ordinal)
            => GetValueAs<string>(ordinal);

        public override bool GetBoolean(int ordinal)
            => GetValueAs<bool>(ordinal);

        public override byte GetByte(int ordinal)
            => GetValueAs<byte>(ordinal);

        public override char GetChar(int ordinal)
            => GetValueAs<char>(ordinal);

        public override short GetInt16(int ordinal)
            => GetValueAs<short>(ordinal);

        public override int GetInt32(int ordinal)
            => GetValueAs<int>(ordinal);

        public override long GetInt64(int ordinal)
            => GetValueAs<long>(ordinal);

        public override float GetFloat(int ordinal)
            => GetValueAs<float>(ordinal);

        public override double GetDouble(int ordinal)
            => GetValueAs<double>(ordinal);

        public override decimal GetDecimal(int ordinal)
            => GetValueAs<decimal>(ordinal);

        public override Guid GetGuid(int ordinal)
            => GetValueAs<Guid>(ordinal);

        public override DateTime GetDateTime(int ordinal)
            => GetValueAs<DateTime>(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            => GetArray(ordinal, dataOffset, buffer, bufferOffset, length);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            => GetArray(ordinal, dataOffset, buffer, bufferOffset, length);

        private long GetArray<TValue>(int ordinal, long dataOffset, TValue[] buffer, int bufferOffset, int maxLength)
            where TValue: struct
        {
            var value = GetValue(ordinal);
            var data  = value as TValue[] ?? ((IEnumerable<TValue>) value).ToArray();
            if (data == null)
                return 0;

            if (buffer == null)
                return data.LongLength;

            if (dataOffset < 0 || dataOffset >= data.LongLength)
                return 0;
            if (bufferOffset < 0 || bufferOffset >= buffer.Length)
                return 0;
            if (maxLength < 0)
                return 0;

            var dataLength   = data.LongLength - dataOffset;
            var bufferLength = buffer.Length - bufferOffset;
            var length       = Math.Min(maxLength, Math.Min(dataLength, bufferLength));

            Array.Copy(data, dataOffset, buffer, bufferOffset, length);
            return length;
        }

        public override int GetValues(object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var length = Math.Min(FieldCount, values.Length);

            for (var i = 0; i < length; i++)
                values[i] = GetValue(i);

            return length;
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }

        public override DataTable GetSchemaTable()
        {
            var schema     = new DataTable();
            var colIsKey   = schema.Columns.Add("IsKey",            typeof(bool)  );
            var colOrdinal = schema.Columns.Add("ColumnOrdinal",    typeof(int)   );
            var colName    = schema.Columns.Add("ColumnName",       typeof(string));
            var colType    = schema.Columns.Add("DataType",         typeof(Type)  );
            var colSize    = schema.Columns.Add("ColumnSize",       typeof(long)  );
            var colPrec    = schema.Columns.Add("NumericPrecision", typeof(byte)  );
            var colScale   = schema.Columns.Add("NumericScale",     typeof(byte)  );

            foreach (var field in _map)
            {
                var type
                    =  Nullable.GetUnderlyingType(field.NetType)
                    ?? field.NetType;

                var row         = schema.NewRow();
                row[colIsKey]   = false;
                row[colOrdinal] = DBNull.Value; // Consumer will fill this in
                row[colName]    = field.Name;
                row[colType]    = type;
                row[colSize]    = DBNull.Value; // Consumer will fill this in
                row[colPrec]    = DBNull.Value; // Consumer will fill this in
                row[colScale]   = DBNull.Value; // Consumer will fill this in
                schema.Rows.Add(row);
            }

            return schema;
        }
    }
}
