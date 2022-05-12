/*
    Copyright 2022 Jeffrey Sharp

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

namespace Sharp.Diagnostics.Logging;

[TestFixture]
public class TraceSourceExtensionsTests : TraceTests
{
    public override TraceSource GetTraceSource()
        => new TraceSource(nameof(TraceSourceExtensionsTests), SourceLevels.All);

    #region Critical

    [Test]
    public void TraceCritical_Message()
    {
        ExpectTraceEvent(Critical, 0, "a");
        Trace.TraceCritical("a");
    }

    [Test]
    public void TraceCritical_IdAndMessage()
    {
        ExpectTraceEvent(Critical, 42, "a");
        Trace.TraceCritical(42, "a");
    }

    [Test]
    public void TraceCritical_Format()
    {
        ExpectTraceEvent(Critical, 0, "{0}", "a");
        Trace.TraceCritical("{0}", "a");
    }

    [Test]
    public void TraceCritical_IdAndFormat()
    {
        ExpectTraceEvent(Critical, 42, "{0}", "a");
        Trace.TraceCritical(42, "{0}", "a");
    }

    [Test]
    public void TraceCritical_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Critical, 0, e);
        Trace.TraceCritical(e);
    }

    [Test]
    public void TraceCritical_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Critical, 42, e);
        Trace.TraceCritical(42, e);
    }

    #endregion
    #region Error

    [Test]
    public void TraceError_Message()
    {
        ExpectTraceEvent(Error, 0, "a");
        Trace.TraceError("a");
    }

    [Test]
    public void TraceError_IdAndMessage()
    {
        ExpectTraceEvent(Error, 42, "a");
        Trace.TraceError(42, "a");
    }

    [Test]
    public void TraceError_Format()
    {
        ExpectTraceEvent(Error, 0, "{0}", "a");
        Trace.TraceError("{0}", "a");
    }

    [Test]
    public void TraceError_IdAndFormat()
    {
        ExpectTraceEvent(Error, 42, "{0}", "a");
        Trace.TraceError(42, "{0}", "a");
    }

    [Test]
    public void TraceError_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Error, 0, e);
        Trace.TraceError(e);
    }

    [Test]
    public void TraceError_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Error, 42, e);
        Trace.TraceError(42, e);
    }

    #endregion
    #region Warning

    [Test]
    public void TraceWarning_Message()
    {
        ExpectTraceEvent(Warning, 0, "a");
        Trace.TraceWarning("a");
    }

    [Test]
    public void TraceWarning_IdAndMessage()
    {
        ExpectTraceEvent(Warning, 42, "a");
        Trace.TraceWarning(42, "a");
    }

    [Test]
    public void TraceWarning_Format()
    {
        ExpectTraceEvent(Warning, 0, "{0}", "a");
        Trace.TraceWarning("{0}", "a");
    }

    [Test]
    public void TraceWarning_IdAndFormat()
    {
        ExpectTraceEvent(Warning, 42, "{0}", "a");
        Trace.TraceWarning(42, "{0}", "a");
    }

    [Test]
    public void TraceWarning_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Warning, 0, e);
        Trace.TraceWarning(e);
    }

    [Test]
    public void TraceWarning_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Warning, 42, e);
        Trace.TraceWarning(42, e);
    }

    #endregion
    #region Information

    [Test]
    public void TraceInformation_Message()
    {
        // NOTE: The additional null parameter deviates from the pattern,
        // because this TraceInformation overload is provided by TraceSource.
        ExpectTraceEvent(Information, 0, "a", null);
        Trace.TraceInformation("a");
    }

    [Test]
    public void TraceInformation_IdAndMessage()
    {
        ExpectTraceEvent(Information, 42, "a");
        Trace.TraceInformation(42, "a");
    }

    [Test]
    public void TraceInformation_Format()
    {
        // NOTE: Provided by TraceSource
        ExpectTraceEvent(Information, 0, "{0}", "a");
        Trace.TraceInformation("{0}", "a");
    }

    [Test]
    public void TraceInformation_IdAndFormat()
    {
        ExpectTraceEvent(Information, 42, "{0}", "a");
        Trace.TraceInformation(42, "{0}", "a");
    }

    [Test]
    public void TraceInformation_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Information, 0, e);
        Trace.TraceInformation(e);
    }

    [Test]
    public void TraceInformation_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Information, 42, e);
        Trace.TraceInformation(42, e);
    }

    #endregion
    #region Verbose

    [Test]
    public void TraceVerbose_Message()
    {
        ExpectTraceEvent(Verbose, 0, "a");
        Trace.TraceVerbose("a");
    }

    [Test]
    public void TraceVerbose_IdAndMessage()
    {
        ExpectTraceEvent(Verbose, 42, "a");
        Trace.TraceVerbose(42, "a");
    }

    [Test]
    public void TraceVerbose_Format()
    {
        ExpectTraceEvent(Verbose, 0, "{0}", "a");
        Trace.TraceVerbose("{0}", "a");
    }

    [Test]
    public void TraceVerbose_IdAndFormat()
    {
        ExpectTraceEvent(Verbose, 42, "{0}", "a");
        Trace.TraceVerbose(42, "{0}", "a");
    }

    [Test]
    public void TraceVerbose_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Verbose, 0, e);
        Trace.TraceVerbose(e);
    }

    [Test]
    public void TraceVerbose_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Verbose, 42, e);
        Trace.TraceVerbose(42, e);
    }

    #endregion
    #region Start

    [Test]
    public void TraceStart_Message()
    {
        ExpectTraceEvent(Start, 0, "a");
        Trace.TraceStart("a");
    }

    [Test]
    public void TraceStart_IdAndMessage()
    {
        ExpectTraceEvent(Start, 42, "a");
        Trace.TraceStart(42, "a");
    }

    [Test]
    public void TraceStart_Format()
    {
        ExpectTraceEvent(Start, 0, "{0}", "a");
        Trace.TraceStart("{0}", "a");
    }

    [Test]
    public void TraceStart_IdAndFormat()
    {
        ExpectTraceEvent(Start, 42, "{0}", "a");
        Trace.TraceStart(42, "{0}", "a");
    }

    [Test]
    public void TraceStart_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Start, 0, e);
        Trace.TraceStart(e);
    }

    [Test]
    public void TraceStart_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Start, 42, e);
        Trace.TraceStart(42, e);
    }

    #endregion
    #region Stop

    [Test]
    public void TraceStop_Message()
    {
        ExpectTraceEvent(Stop, 0, "a");
        Trace.TraceStop("a");
    }

    [Test]
    public void TraceStop_IdAndMessage()
    {
        ExpectTraceEvent(Stop, 42, "a");
        Trace.TraceStop(42, "a");
    }

    [Test]
    public void TraceStop_Format()
    {
        ExpectTraceEvent(Stop, 0, "{0}", "a");
        Trace.TraceStop("{0}", "a");
    }

    [Test]
    public void TraceStop_IdAndFormat()
    {
        ExpectTraceEvent(Stop, 42, "{0}", "a");
        Trace.TraceStop(42, "{0}", "a");
    }

    [Test]
    public void TraceStop_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Stop, 0, e);
        Trace.TraceStop(e);
    }

    [Test]
    public void TraceStop_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Stop, 42, e);
        Trace.TraceStop(42, e);
    }

    #endregion
    #region Suspend

    [Test]
    public void TraceSuspend_Message()
    {
        ExpectTraceEvent(Suspend, 0, "a");
        Trace.TraceSuspend("a");
    }

    [Test]
    public void TraceSuspend_IdAndMessage()
    {
        ExpectTraceEvent(Suspend, 42, "a");
        Trace.TraceSuspend(42, "a");
    }

    [Test]
    public void TraceSuspend_Format()
    {
        ExpectTraceEvent(Suspend, 0, "{0}", "a");
        Trace.TraceSuspend("{0}", "a");
    }

    [Test]
    public void TraceSuspend_IdAndFormat()
    {
        ExpectTraceEvent(Suspend, 42, "{0}", "a");
        Trace.TraceSuspend(42, "{0}", "a");
    }

    [Test]
    public void TraceSuspend_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Suspend, 0, e);
        Trace.TraceSuspend(e);
    }

    [Test]
    public void TraceSuspend_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Suspend, 42, e);
        Trace.TraceSuspend(42, e);
    }

    #endregion
    #region Resume

    [Test]
    public void TraceResume_Message()
    {
        ExpectTraceEvent(Resume, 0, "a");
        Trace.TraceResume("a");
    }

    [Test]
    public void TraceResume_IdAndMessage()
    {
        ExpectTraceEvent(Resume, 42, "a");
        Trace.TraceResume(42, "a");
    }

    [Test]
    public void TraceResume_Format()
    {
        ExpectTraceEvent(Resume, 0, "{0}", "a");
        Trace.TraceResume("{0}", "a");
    }

    [Test]
    public void TraceResume_IdAndFormat()
    {
        ExpectTraceEvent(Resume, 42, "{0}", "a");
        Trace.TraceResume(42, "{0}", "a");
    }

    [Test]
    public void TraceResume_Exception()
    {
        var e = new Exception("a");
        ExpectTraceData(Resume, 0, e);
        Trace.TraceResume(e);
    }

    [Test]
    public void TraceResume_IdAndException()
    {
        var e = new Exception("a");
        ExpectTraceData(Resume, 42, e);
        Trace.TraceResume(42, e);
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
        Trace.TraceTransfer(   "a", id);
    }

    [Test]
    public void Transfer_IdAndMessageAndActivityId()
    {
        // NOTE: Provided by TraceSource
        var id = Guid.NewGuid();
        ExpectTraceTransfer(42, "a", id);
        Trace.TraceTransfer(42, "a", id);
    }

    #endregion
    #region Event

    [Test]
    public void TraceEvent_Message()
    {
        ExpectTraceEvent(Verbose, 0, "a");
        Trace.TraceEvent(Verbose,    "a");
    }

    [Test]
    public void TraceEvent_IdAndMessage()
    {
        // NOTE: Provided by TraceSource
        ExpectTraceEvent(Verbose, 42, "a");
        Trace.TraceEvent(Verbose, 42, "a");
    }

    [Test]
    public void TraceEvent_Format()
    {
        ExpectTraceEvent(Verbose, 0, "{0}", "a");
        Trace.TraceEvent(Verbose,    "{0}", "a");
    }

    [Test]
    public void TraceEvent_IdAndFormat()
    {
        // NOTE: Provided by TraceSource
        ExpectTraceEvent(Verbose, 42, "{0}", "a");
        Trace.TraceEvent(Verbose, 42, "{0}", "a");
    }

    #endregion
    #region Data

    [Test]
    public void TraceData_Object()
    {
        var o = new object();
        ExpectTraceData(Verbose, 0, o);
        Trace.TraceData(Verbose,    o);
    }

    [Test]
    public void TraceData_IdAndObject()
    {
        // NOTE: Provided by TraceSource
        var o = new object();
        ExpectTraceData(Verbose, 42, o);
        Trace.TraceData(Verbose, 42, o);
    }

    [Test]
    public void TraceData_ObjectArray()
    {
        var a = new object();
        var b = new object();
        ExpectTraceData(Verbose, 0, a, b);
        Trace.TraceData(Verbose,    a, b);
    }

    [Test]
    public void TraceData_IdAndObjectArray()
    {
        // NOTE: Provided by TraceSource
        var a = new object();
        var b = new object();
        ExpectTraceData(Verbose, 42, a, b);
        Trace.TraceData(Verbose, 42, a, b);
    }

    #endregion
}
