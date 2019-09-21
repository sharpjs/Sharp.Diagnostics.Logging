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
            private readonly string _name;
            private readonly string _dbType;

            protected Field(string name, string dbType)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                if (dbType == null)
                    throw new ArgumentNullException(nameof(dbType));

                _name   = name;
                _dbType = dbType;
            }

            public string Name   => _name;
            public string DbType => _dbType;

            public abstract Type NetType { get; }

            public object GetValue(T obj)
            {
                return GetValueAsObject(obj);
            }

            public TValue GetValueAs<TValue>(T obj)
            {
                var typed = this as Field<TValue>;
                var ok    = typed != null;
                return ok
                    ? typed.GetValue(obj)
                    : (TValue) GetValueAsObject(obj);
            }

            public bool TryGetValueAs<TValue>(T obj, out TValue value)
            {
                var typed = this as Field<TValue>;
                var ok    = typed != null;
                value     = ok
                    ? typed.GetValue(obj)
                    : default(TValue);
                return ok;
            }

            protected abstract object GetValueAsObject(T obj);
        }

        public class Field<TValue> : Field
        {
            private readonly Func<T, TValue> _getter;

            public Field(string name, string dbType, Func<T, TValue> getter)
                : base(name, dbType)
            {
                if (getter == null)
                    throw new ArgumentNullException(nameof(getter));

                _getter = getter;
            }

            public override Type NetType => typeof(TValue);

            public new TValue GetValue(T obj)
            {
                return _getter(obj);
            }

            protected override object GetValueAsObject(T obj)
            {
                return _getter(obj);
            }
        }
    }
}
