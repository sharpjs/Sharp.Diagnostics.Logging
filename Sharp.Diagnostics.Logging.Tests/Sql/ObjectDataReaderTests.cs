using System;
using FluentAssertions;
using NUnit.Framework;

namespace Sharp.Data
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ObjectDataReaderTests
    {
        [Test]
        public void IsDBNull_ReferenceType_True()
        {
            var reader = FakeReader(m => m
                .Field("any", "any", _ => null as byte[])
                , 0
            );

            using (reader)
            {
                reader.Read()     .Should().BeTrue();
                reader.IsDBNull(0).Should().BeTrue();
            }
        }

        [Test]
        public void IsDBNull_ReferenceType_False()
        {
            var reader = FakeReader(m => m
                .Field("any", "any", _ => new byte[0])
                , 0
            );

            using (reader)
            {
                reader.Read()     .Should().BeTrue();
                reader.IsDBNull(0).Should().BeFalse();
            }
        }
        [Test]
        public void IsDBNull_NullableValueType_True()
        {
            var reader = FakeReader(m => m
                .Field("any", "any", _ => null as int?)
                , 0
            );

            using (reader)
            {
                reader.Read()     .Should().BeTrue();
                reader.IsDBNull(0).Should().BeTrue();
            }
        }

        [Test]
        public void IsDBNull_NullableValueType_False()
        {
            var reader = FakeReader(m => m
                .Field("any", "any", _ => 42 as int?)
                , 0
            );

            using (reader)
            {
                reader.Read()     .Should().BeTrue();
                reader.IsDBNull(0).Should().BeFalse();
            }
        }

        [Test]
        public void IsDBNull_NonNullableValueType_False()
        {
            var reader = FakeReader(m => m
                .Field("any", "any", _ => 42)
                , 0
            );

            using (reader)
            {
                reader.Read()     .Should().BeTrue();
                reader.IsDBNull(0).Should().BeFalse();
            }
        }

        [Test]
        public void GetValue_Null()
        {
            var reader = FakeReader(null as string);

            using (reader)
            {
                reader.Read().Should().BeTrue();

                reader.GetValue(0).Should().Be(DBNull.Value);
            }
        }

        [Test]
        public void TypicalReader()
        {
            var foos = new[]
            {
                new Foo { A = "x", B = 13 },
                new Foo { A = "y", B = 42 },
            };

            var reader = new ObjectDataReader<Foo>(foos, Foo.Map);
            reader.HasRows        .Should().BeTrue();
            reader.FieldCount     .Should().Be(2);
            reader.Depth          .Should().Be(0);
            reader.RecordsAffected.Should().Be(-1);

            reader.Read()         .Should().BeTrue();
            reader.GetString(0)   .Should().Be("x");
            reader.GetInt32(1)    .Should().Be(13);

            reader.Read()         .Should().BeTrue();
            reader.GetString(0)   .Should().Be("y");
            reader.GetInt32(1)    .Should().Be(42);

            reader.Read()         .Should().BeFalse();
        }

        private static ObjectDataReader<T> FakeReader<T>(params T[] items)
        {
            return FakeReader(m => m.Field("any", "any", x => x), items);
        }

        private static ObjectDataReader<T> FakeReader<T>(
            Action<ObjectDataMap<T>.Builder> build,
            params T[]                       items)
        {
            var map = new ObjectDataMap<T>(build);
            return new ObjectDataReader<T>(items, map);
        }

        public class Foo
        {
            public string A;
            public int    B;

            internal static readonly ObjectDataMap<Foo>
                Map = new ObjectDataMap<Foo>(b => b
                    .Field("A", "nvarchar(50)", x => x.A)
                    .Field("B", "int",          x => x.B)
                );
        }
    }
}
