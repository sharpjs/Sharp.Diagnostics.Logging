using System;
using FluentAssertions;
using NUnit.Framework;
using static FluentAssertions.FluentActions;

namespace Sharp.Data
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ObjectDataMapTests
    {
        [Test]
        public void EmptyMap()
        {
            var map = new ObjectDataMap<Foo>(b => { });

            map.Count.Should().Be(0);
            map.Invoking(m => m[ 0]).Should().Throw<IndexOutOfRangeException>();
            map.Invoking(m => m[-1]).Should().Throw<IndexOutOfRangeException>();
        }

        [Test]
        public void TypicalMap()
        {
            var map = new ObjectDataMap<Foo>(b => b
                .Field("A", "nvarchar(50)", x => x.A)
                .Field("B", "int",          x => x.B)
            );

            map.Count     .Should().Be(2);
            map[0].Name   .Should().Be("A");
            map[0].DbType .Should().Be("nvarchar(50)");
            map[0].NetType.Should().Be(typeof(string));
            map[1].Name   .Should().Be("B");
            map[1].DbType .Should().Be("int");
            map[1].NetType.Should().Be(typeof(int));

            map.Invoking(m => { var _ = m[2]; }).Should().Throw<IndexOutOfRangeException>();
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
