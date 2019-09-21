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

namespace Sharp.Diagnostics.Logging.Sql
{
    internal class ObjectDataMap<T> : IEnumerable<ObjectDataMap<T>.Field>
    {
        private readonly Field[] _fields;

        public ObjectDataMap(Action<Builder> build)
        {
            var builder = new Builder();
            build(builder);
            _fields = builder.Complete();
        }

        public int Count => _fields.Length;

        public Field this[int ordinal] => _fields[ordinal];

        public int GetOrdinal(string name)
        {
            var ordinal = Array.FindIndex(_fields, f => f.Name == name);
            if (ordinal >= 0)
                return ordinal;

            throw new IndexOutOfRangeException(
                "The specified name is not a valid column name."
            );
        }

        public IEnumerator<Field> GetEnumerator()
        {
            return (_fields as IList<Field>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        public class Builder
        {
            private readonly List<Field> _fields = new List<Field>();

            public Builder Field<TField>(string name, string dbType, Func<T, TField> getter)
            {
                _fields.Add(new Field<TField>(name, dbType, getter));
                return this;
            }

            internal Field[] Complete()
            {
                return _fields.ToArray();
            }
        }

        public abstract class Field
        {
            protected Field(string name, string dbType)
            {
                Name   = name   ?? throw new ArgumentNullException(nameof(name));
                DbType = dbType ?? throw new ArgumentNullException(nameof(dbType));
            }

            public string Name { get; }

            public string DbType { get; }

            public abstract Type NetType { get; }

            public object GetValue(T obj)
            {
                return GetValueAsObject(obj);
            }

            public TValue GetValueAs<TValue>(T obj)
            {
                if (this is Field<TValue> typed)
                    return typed.GetValue(obj);

                throw new NotSupportedException(string.Format(
                    "Cannot get field {0} value as type {1}.  " +
                    "The value must be accessed as type {2}.",
                    Name, typeof(TValue), NetType
                ));
            }

            public bool TryGetValueAs<TValue>(T obj, out TValue value)
            {
                if (this is Field<TValue> typed)
                {
                    value = typed.GetValue(obj);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }

            protected abstract object GetValueAsObject(T obj);
        }

        public class Field<TValue> : Field
        {
            private readonly Func<T, TValue> _getter;

            public Field(string name, string dbType, Func<T, TValue> getter)
                : base(name, dbType)
            {
                _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            }

            public override Type NetType
                => typeof(TValue);

            public new TValue GetValue(T obj)
                => _getter(obj);

            protected override object GetValueAsObject(T obj)
                => _getter(obj);
        }
    }
}
