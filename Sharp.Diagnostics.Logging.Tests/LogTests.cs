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
using FluentAssertions;
using NUnit.Framework;
using static System.Diagnostics.TraceEventType;

namespace Sharp.Diagnostics.Logging
{
    [TestFixture]
    [SingleThreaded] // because a single static TraceSource is required
    public class LogTests : TraceTests
    {
        private static readonly TraceSource _Trace
            = new TraceSource(nameof(LogTests), SourceLevels.All);

        public override TraceSource GetTraceSource()
            => _Trace;

        private abstract class Log : Log<LogTests> { }

        #region Critical

        [Test]
        public void Critical_Message()
        {
            ExpectTraceEvent(Critical, 0, "a");
            Log.Critical("a");
        }

        [Test]
        public void Critical_IdAndMessage()
        {
            ExpectTraceEvent(Critical, 42, "a");
            Log.Critical(42, "a");
        }

        [Test]
        public void Critical_Format()
        {
            ExpectTraceEvent(Critical, 0, "{0}", "a");
            Log.Critical("{0}", "a");
        }

        [Test]
        public void Critical_IdAndFormat()
        {
            ExpectTraceEvent(Critical, 42, "{0}", "a");
            Log.Critical(42, "{0}", "a");
        }

        [Test]
        public void Critical_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Critical, 0, e);
            Log.Critical(e);
        }

        [Test]
        public void Critical_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Critical, 42, e);
            Log.Critical(42, e);
        }

        #endregion
        #region Error

        [Test]
        public void Error_Message()
        {
            ExpectTraceEvent(Error, 0, "a");
            Log.Error("a");
        }

        [Test]
        public void Error_IdAndMessage()
        {
            ExpectTraceEvent(Error, 42, "a");
            Log.Error(42, "a");
        }

        [Test]
        public void Error_Format()
        {
            ExpectTraceEvent(Error, 0, "{0}", "a");
            Log.Error("{0}", "a");
        }

        [Test]
        public void Error_IdAndFormat()
        {
            ExpectTraceEvent(Error, 42, "{0}", "a");
            Log.Error(42, "{0}", "a");
        }

        [Test]
        public void Error_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Error, 0, e);
            Log.Error(e);
        }

        [Test]
        public void Error_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Error, 42, e);
            Log.Error(42, e);
        }

        #endregion
        #region Warning

        [Test]
        public void Warning_Message()
        {
            ExpectTraceEvent(Warning, 0, "a");
            Log.Warning("a");
        }

        [Test]
        public void Warning_IdAndMessage()
        {
            ExpectTraceEvent(Warning, 42, "a");
            Log.Warning(42, "a");
        }

        [Test]
        public void Warning_Format()
        {
            ExpectTraceEvent(Warning, 0, "{0}", "a");
            Log.Warning("{0}", "a");
        }

        [Test]
        public void Warning_IdAndFormat()
        {
            ExpectTraceEvent(Warning, 42, "{0}", "a");
            Log.Warning(42, "{0}", "a");
        }

        [Test]
        public void Warning_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Warning, 0, e);
            Log.Warning(e);
        }

        [Test]
        public void Warning_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Warning, 42, e);
            Log.Warning(42, e);
        }

        #endregion
        #region Information

        [Test]
        public void Information_Message()
        {
            // NOTE: The additional null parameter deviates from the pattern, because the
            // underlying TraceInformation overload is provided by TraceSource.
            ExpectTraceEvent(Information, 0, "a", null);
            Log.Information("a");
        }

        [Test]
        public void Information_IdAndMessage()
        {
            ExpectTraceEvent(Information, 42, "a");
            Log.Information(42, "a");
        }

        [Test]
        public void Information_Format()
        {
            ExpectTraceEvent(Information, 0, "{0}", "a");
            Log.Information("{0}", "a");
        }

        [Test]
        public void Information_IdAndFormat()
        {
            ExpectTraceEvent(Information, 42, "{0}", "a");
            Log.Information(42, "{0}", "a");
        }

        [Test]
        public void Information_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Information, 0, e);
            Log.Information(e);
        }

        [Test]
        public void Information_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Information, 42, e);
            Log.Information(42, e);
        }

        #endregion
        #region Verbose

        [Test]
        public void Verbose_Message()
        {
            ExpectTraceEvent(Verbose, 0, "a");
            Log.Verbose("a");
        }

        [Test]
        public void Verbose_IdAndMessage()
        {
            ExpectTraceEvent(Verbose, 42, "a");
            Log.Verbose(42, "a");
        }

        [Test]
        public void Verbose_Format()
        {
            ExpectTraceEvent(Verbose, 0, "{0}", "a");
            Log.Verbose("{0}", "a");
        }

        [Test]
        public void Verbose_IdAndFormat()
        {
            ExpectTraceEvent(Verbose, 42, "{0}", "a");
            Log.Verbose(42, "{0}", "a");
        }

        [Test]
        public void Verbose_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Verbose, 0, e);
            Log.Verbose(e);
        }

        [Test]
        public void Verbose_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Verbose, 42, e);
            Log.Verbose(42, e);
        }

        #endregion
        #region Start

        [Test]
        public void Start_Message()
        {
            ExpectTraceEvent(Start, 0, "a");
            Log.Start("a");
        }

        [Test]
        public void Start_IdAndMessage()
        {
            ExpectTraceEvent(Start, 42, "a");
            Log.Start(42, "a");
        }

        [Test]
        public void Start_Format()
        {
            ExpectTraceEvent(Start, 0, "{0}", "a");
            Log.Start("{0}", "a");
        }

        [Test]
        public void Start_IdAndFormat()
        {
            ExpectTraceEvent(Start, 42, "{0}", "a");
            Log.Start(42, "{0}", "a");
        }

        [Test]
        public void Start_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Start, 0, e);
            Log.Start(e);
        }

        [Test]
        public void Start_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Start, 42, e);
            Log.Start(42, e);
        }

        #endregion
        #region Stop

        [Test]
        public void Stop_Message()
        {
            ExpectTraceEvent(Stop, 0, "a");
            Log.Stop("a");
        }

        [Test]
        public void Stop_IdAndMessage()
        {
            ExpectTraceEvent(Stop, 42, "a");
            Log.Stop(42, "a");
        }

        [Test]
        public void Stop_Format()
        {
            ExpectTraceEvent(Stop, 0, "{0}", "a");
            Log.Stop("{0}", "a");
        }

        [Test]
        public void Stop_IdAndFormat()
        {
            ExpectTraceEvent(Stop, 42, "{0}", "a");
            Log.Stop(42, "{0}", "a");
        }

        [Test]
        public void Stop_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Stop, 0, e);
            Log.Stop(e);
        }

        [Test]
        public void Stop_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Stop, 42, e);
            Log.Stop(42, e);
        }

        #endregion
        #region Suspend

        [Test]
        public void Suspend_Message()
        {
            ExpectTraceEvent(Suspend, 0, "a");
            Log.Suspend("a");
        }

        [Test]
        public void Suspend_IdAndMessage()
        {
            ExpectTraceEvent(Suspend, 42, "a");
            Log.Suspend(42, "a");
        }

        [Test]
        public void Suspend_Format()
        {
            ExpectTraceEvent(Suspend, 0, "{0}", "a");
            Log.Suspend("{0}", "a");
        }

        [Test]
        public void Suspend_IdAndFormat()
        {
            ExpectTraceEvent(Suspend, 42, "{0}", "a");
            Log.Suspend(42, "{0}", "a");
        }

        [Test]
        public void Suspend_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Suspend, 0, e);
            Log.Suspend(e);
        }

        [Test]
        public void Suspend_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Suspend, 42, e);
            Log.Suspend(42, e);
        }

        #endregion
        #region Resume

        [Test]
        public void Resume_Message()
        {
            ExpectTraceEvent(Resume, 0, "a");
            Log.Resume("a");
        }

        [Test]
        public void Resume_IdAndMessage()
        {
            ExpectTraceEvent(Resume, 42, "a");
            Log.Resume(42, "a");
        }

        [Test]
        public void Resume_Format()
        {
            ExpectTraceEvent(Resume, 0, "{0}", "a");
            Log.Resume("{0}", "a");
        }

        [Test]
        public void Resume_IdAndFormat()
        {
            ExpectTraceEvent(Resume, 42, "{0}", "a");
            Log.Resume(42, "{0}", "a");
        }

        [Test]
        public void Resume_Exception()
        {
            var e = new Exception("a");
            ExpectTraceData(Resume, 0, e);
            Log.Resume(e);
        }

        [Test]
        public void Resume_IdAndException()
        {
            var e = new Exception("a");
            ExpectTraceData(Resume, 42, e);
            Log.Resume(42, e);
        }

        #endregion
        #region Operations / Correlation

        [Test]
        public void Operation()
        {
            ExpectTraceOperation("foo");
            using (var operation = Trace.Operation("foo"))
                operation.Should().BeOfType<TraceOperation>();
        }

        [Test]
        public void Do_Action()
        {
            ExpectTraceOperation("foo");
            var count = 0;
            Trace.Do("foo", () => { count++; });
            count.Should().Be(1);
        }

        [Test]
        public void Do_Func()
        {
            ExpectTraceOperation("foo");
            var result = Trace.Do("foo", () => 42);
            result.Should().Be(42);
        }

        [Test]
        public void Transfer_MessageAndActivityId()
        {
            var id = Guid.NewGuid();
            ExpectTraceTransfer(0, "a", id);
            Log.Transfer("a", id);
        }

        [Test]
        public void Transfer_IdAndMessageAndActivityId()
        {
            // NOTE: Provided by TraceSource
            var id = Guid.NewGuid();
            ExpectTraceTransfer(42, "a", id);
            Log.Transfer(42, "a", id);
        }

        #endregion
        #region Event

        [Test]
        public void Event_Message()
        {
            ExpectTraceEvent(Verbose, 0, "a");
            Log.Event(Verbose, "a");
        }

        [Test]
        public void Event_IdAndMessage()
        {
            ExpectTraceEvent(Verbose, 42, "a");
            Log.Event(Verbose, 42, "a");
        }

        [Test]
        public void Event_Format()
        {
            ExpectTraceEvent(Verbose, 0, "{0}", "a");
            Log.Event(Verbose, "{0}", "a");
        }

        [Test]
        public void Event_IdAndFormat()
        {
            ExpectTraceEvent(Verbose, 42, "{0}", "a");
            Log.Event(Verbose, 42, "{0}", "a");
        }

        #endregion
        #region Data

        [Test]
        public void Data_Object()
        {
            var o = new object();
            ExpectTraceData(Verbose, 0, o);
            Log.Data(Verbose, o);
        }

        [Test]
        public void Data_IdAndObject()
        {
            var o = new object();
            ExpectTraceData(Verbose, 42, o);
            Log.Data(Verbose, 42, o);
        }

        [Test]
        public void Data_ObjectArray()
        {
            var a = new object();
            var b = new object();
            ExpectTraceData(Verbose, 0, a, b);
            Log.Data(Verbose, a, b);
        }

        [Test]
        public void Data_IdAndObjectArray()
        {
            var a = new object();
            var b = new object();
            ExpectTraceData(Verbose, 42, a, b);
            Log.Data(Verbose, 42, a, b);
        }

        #endregion
    }
}
