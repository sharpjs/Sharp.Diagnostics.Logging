/*
    Copyright (C) 2018 Jeffrey Sharp

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
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;
using static System.Diagnostics.TraceEventType;

namespace Sharp.Diagnostics.Logging
{
    [TestFixture]
    public class TraceOperationTests : TraceTests
    {
        public override TraceSource GetTraceSource()
            => new TraceSource(nameof(TraceOperationTests), SourceLevels.All);

        [Test]
        public void Construct_TraceSource()
        {
            ExpectTraceOperation("a");

            using (var op = new TraceOperation(Trace, "a")) { }
        }

        [Test]
        public void Construct_StaticTrace()
        {
            UseStaticTraceApi();
            ExpectTraceOperation("a");

            using (var op = new TraceOperation("a")) { }
        }

        [Test]
        public void Name_Default()
        {
            ExpectTraceOperation(nameof(Name_Default));

            using (var op = new TraceOperation(Trace))
            {
                op.Name.Should().Be(nameof(Name_Default));
            }
        }

        [Test]
        public void Name_Explicit()
        {
            ExpectTraceOperation("Test");

            using (var op = new TraceOperation(Trace, "Test"))
            {
                op.Name.Should().Be("Test");
            }
        }

        [Test]
        public void Name_Null()
        {
            // No expected tracing

            using (var op = new TraceOperation(Trace, null))
            {
                op.Name.Should().BeNull();
            }
        }

        [Test]
        public void StartTime()
        {
            ExpectTraceOperation("Test");

            var min = DateTime.UtcNow;

            using (var op = new TraceOperation(Trace, "Test"))
            {
                var max = DateTime.UtcNow;

                var value = op.StartTime;

                value.Should().BeOnOrAfter (min);
                value.Should().BeOnOrBefore(max);
            }
        }

        [Test]
        public void Elapsed()
        {
            ExpectTraceOperation("Test");

            var before = DateTime.UtcNow;

            using (var op = new TraceOperation(Trace, "Test"))
            {
                var after = DateTime.UtcNow;

                Thread.Sleep(20.Milliseconds());

                var min   = DateTime.UtcNow - after;
                var value = op.ElapsedTime;
                var max   = DateTime.UtcNow - before;

                value.Should().BeGreaterOrEqualTo(min);
                value.Should().BeLessOrEqualTo   (max);
            }
        }

        [Test]
        public void Exception_Default()
        {
            ExpectTraceOperation("Test");

            using (var op = new TraceOperation(Trace, "Test"))
            {
                op.Exception.Should().BeNull();
            }
        }

        [Test]
        public void Exception_Explicit()
        {
            var e = new ApplicationException("Something bad happened.");

            ExpectTraceOperation("Test");
            ExpectTraceData(Error, 0, e);

            using (var op = new TraceOperation(Trace, "Test"))
            {
                op.Exception = e;
                op.Exception.Should().BeSameAs(e);
            }
        }

        [Test]
        public void Do_ActionWithStaticTrace()
        {
            var count = 0;
            UseStaticTraceApi();
            ExpectTraceOperation("Test");

            TraceOperation.Do("Test", () => { count++; });

            count.Should().Be(1);
        }

        [Test]
        public void Do_ActionWithStaticTrace_NullAction()
        {
            Invoking(() => TraceOperation.Do("Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Do_ActionWithStaticTrace_Exception()
        {
            var e = CreateTestException();

            UseStaticTraceApi();
            ExpectTraceOperation("Test");
            ExpectTraceEvent(Error, 0, s => s.Contains(e.Message));

            this.Invoking(_ => TraceOperation.Do("Test", () => { throw e; }))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public void Do_ActionWithTraceSource()
        {
            var count = 0;
            ExpectTraceOperation("Test");

            TraceOperation.Do(Trace, "Test", () => { count++; });

            count.Should().Be(1);
        }

        [Test]
        public void Do_ActionWithTraceSource_NullAction()
        {
            Invoking(() => TraceOperation.Do(Trace, "Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Do_ActionWithTraceSource_Exception()
        {
            var e = CreateTestException();

            ExpectTraceOperation("Test");
            ExpectTraceData(Error, 0, e);

            this.Invoking(_ => TraceOperation.Do(Trace, "Test", () => { throw e; }))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public void Do_FuncWithStaticTrace()
        {
            UseStaticTraceApi();
            ExpectTraceOperation("Test");

            var result = TraceOperation.Do("Test", () => 42);

            result.Should().Be(42);
        }

        [Test]
        public void Do_FuncWithStaticTrace_NullAction()
        {
            Invoking(() => TraceOperation.Do<object>("Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Do_FuncWithStaticTrace_Exception()
        {
            var e = CreateTestException();

            UseStaticTraceApi();
            ExpectTraceOperation("Test");
            ExpectTraceEvent(Error, 0, s => s.Contains(e.Message));

            this.Invoking(_ => TraceOperation.Do("Test", () => null as int? ?? throw e))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public void Do_FuncWithTraceSource()
        {
            ExpectTraceOperation("Test");

            var result = TraceOperation.Do(Trace, "Test", () => 42);

            result.Should().Be(42);
        }

        [Test]
        public void Do_FuncWithTraceSource_NullAction()
        {
            Invoking(() => TraceOperation.Do<object>(Trace, "Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Do_FuncWithTraceSource_Exception()
        {
            var e = CreateTestException();

            ExpectTraceOperation("Test");
            ExpectTraceData(Error, 0, e);

            this.Invoking(_ => TraceOperation.Do(Trace, "Test", () => null as int? ?? throw e))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public async Task DoAsync_ActionWithStaticTrace()
        {
            var count = 0;
            UseStaticTraceApi();
            ExpectTraceOperation("Test");

            await TraceOperation.DoAsync("Test", () => { count++; return Task.CompletedTask; });

            count.Should().Be(1);
        }

        [Test]
        public void DoAsync_ActionWithStaticTrace_NullAction()
        {
            Awaiting(() => TraceOperation.DoAsync("Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void DoAsync_ActionWithStaticTrace_Exception()
        {
            var e = CreateTestException();

            UseStaticTraceApi();
            ExpectTraceOperation("Test");
            ExpectTraceEvent(Error, 0, s => s.Contains(e.Message));

            this.Awaiting(_ => TraceOperation.DoAsync("Test", () => { throw e; }))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public async Task DoAsync_ActionWithTraceSource()
        {
            var count = 0;
            ExpectTraceOperation("Test");

            await TraceOperation.DoAsync(Trace, "Test", () => { count++; return Task.CompletedTask; });

            count.Should().Be(1);
        }

        [Test]
        public void DoAsync_ActionWithTraceSource_NullAction()
        {
            Awaiting(() => TraceOperation.DoAsync(Trace, "Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void DoAsync_ActionWithTraceSource_Exception()
        {
            var e = CreateTestException();

            ExpectTraceOperation("Test");
            ExpectTraceData(Error, 0, e);

            this.Awaiting(_ => TraceOperation.DoAsync(Trace, "Test", () => { throw e; }))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public async Task DoAsync_FuncWithStaticTrace()
        {
            UseStaticTraceApi();
            ExpectTraceOperation("Test");

            var result = await TraceOperation.DoAsync("Test", () => Task.FromResult(42));

            result.Should().Be(42);
        }

        [Test]
        public void DoAsync_FuncWithStaticTrace_NullAction()
        {
            Awaiting(() => TraceOperation.DoAsync<object>("Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void DoAsync_FuncWithStaticTrace_Exception()
        {
            var e = CreateTestException();

            UseStaticTraceApi();
            ExpectTraceOperation("Test");
            ExpectTraceEvent(Error, 0, s => s.Contains(e.Message));

            this.Awaiting(_ => TraceOperation.DoAsync("Test", () => Task.FromResult(null as int? ?? throw e)))
                .Should().Throw<ApplicationException>();
        }

        [Test]
        public async Task DoAsync_FuncWithTraceSource()
        {
            ExpectTraceOperation("Test");

            var result = await TraceOperation.DoAsync(Trace, "Test", () => Task.FromResult(42));

            result.Should().Be(42);
        }

        [Test]
        public void DoAsync_FuncWithTraceSource_NullAction()
        {
            Awaiting(() => TraceOperation.DoAsync<object>(Trace, "Test", null!))
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void DoAsync_FuncWithTraceSource_Exception()
        {
            var e = CreateTestException();

            ExpectTraceOperation("Test");
            ExpectTraceData(Error, 0, e);

            this.Awaiting(_ => TraceOperation.DoAsync(Trace, "Test", () => Task.FromResult(null as int? ?? throw e)))
                .Should().Throw<ApplicationException>();
        }
    }
}
