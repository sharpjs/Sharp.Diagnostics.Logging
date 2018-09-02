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
using Moq;
using NUnit.Framework;
using static System.Diagnostics.TraceEventType;

namespace Sharp.Diagnostics.Logging
{
    public abstract class TraceTests : ITraceSourceProvider
    {
        protected Mock<TraceListener> Listener { get; private set; }
        protected TraceSource         Trace    { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            Listener = new Mock<TraceListener>(MockBehavior.Strict);

            Trace = GetTraceSource();
            Trace.Listeners.Clear();
            Trace.Listeners.Add(Listener.Object);
        }

        [TearDown]
        public virtual void TearDown()
        {
            Listener.Verify();
        }

        public abstract TraceSource GetTraceSource();

        protected void ExpectTraceEvent(TraceEventType type, int id, string message)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    type, id, message
                ))
                .Verifiable();
        }

        protected void ExpectTraceEvent(TraceEventType type, int id, string format, params object[] args)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    type, id, format, args
                ))
                .Verifiable();
        }

        protected void ExpectTraceData(TraceEventType type, int id, object data)
        {
            Listener
                .Setup(t => t.TraceData(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    type, id, data
                ))
                .Verifiable();
        }

        protected void ExpectTraceData(TraceEventType type, int id, params object[] data)
        {
            Listener
                .Setup(t => t.TraceData(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    type, id, data
                ))
                .Verifiable();
        }

        protected void ExpectTraceOperation(string name)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    Information, 0,
                    It.Is<string>(s => s.IndexOf("Starting", StringComparison.Ordinal) >= 0),
                    It.IsAny<object[]>()
                ))
                .Verifiable();

            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    Information, 0,
                    It.Is<string>(s => s.IndexOf("Completed", StringComparison.Ordinal) >= 0),
                    It.IsAny<object[]>()
                ))
                .Verifiable();
        }

        protected void ExpectTraceTransfer(int id, string message, Guid activityId)
        {
            Listener
                .Setup(t => t.TraceTransfer(
                    It.IsNotNull<TraceEventCache>(),
                    Trace.Name,
                    id, message, activityId
                ))
                .Verifiable();
        }
    }
}
