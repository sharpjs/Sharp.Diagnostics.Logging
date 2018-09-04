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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Moq;
using NUnit.Framework;
using static System.Diagnostics.TraceEventType;
using StaticTrace = System.Diagnostics.Trace;

#if NETCOREAPP
using System.Reflection;
#endif

namespace Sharp.Diagnostics.Logging
{
    public abstract class TraceTests : ITraceSourceProvider
    {
        protected Mock<TraceListener>     Listener      { get; private set; }
        protected TraceSource             Trace         { get; private set; }

        protected bool                    IsStaticTrace { get; private set; }
        protected string                  TraceName     { get; private set; }
        protected TraceListenerCollection Listeners     { get; private set; }

        private static readonly object
            StaticTraceLock = new object();

        [SetUp]
        public virtual void SetUp()
        {
            Listener = new Mock<TraceListener>(MockBehavior.Strict);
            Trace    = GetTraceSource();

            SetApi(Trace.Name, Trace.Listeners, isStatic: false);
        }

        [TearDown]
        public virtual void TearDown()
        {
            Listeners.Clear();

            if (IsStaticTrace)
                Monitor.Exit(StaticTraceLock);

            Listener.Verify();
        }

        public abstract TraceSource GetTraceSource();

        // Called never or once by test
        protected void UseStaticTraceApi()
        {
            #if NETCOREAPP
                var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            #else
                var appName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            #endif

            Listeners.Clear();

            // Tests using the static trace API must be single-threaded
            Monitor.Enter(StaticTraceLock);

            SetApi(appName, StaticTrace.Listeners, isStatic: true);
        }

        private void SetApi(string name, TraceListenerCollection listeners, bool isStatic)
        {
            IsStaticTrace = isStatic;
            TraceName     = name;
            Listeners     = listeners;

            Listeners.Clear();
            Listeners.Add(Listener.Object);
        }

        protected void ExpectTraceEvent(TraceEventType type, int id)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id
                ))
                .Verifiable();
        }

        protected void ExpectTraceEvent(TraceEventType type, int id, string message)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id, message
                ))
                .Verifiable();
        }

        protected void ExpectTraceEvent(TraceEventType type, int id,
            Expression<Func<string, bool>> message)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id, It.Is(message)
                ))
                .Verifiable();
        }

        protected void ExpectTraceEvent(TraceEventType type, int id, string format, params object[] args)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id, format, args
                ))
                .Verifiable();
        }

        protected void ExpectTraceEvent(TraceEventType type, int id,
            Expression<Func<string,   bool>> format,
            Expression<Func<object[], bool>> args)
        {
            Listener
                .Setup(t => t.TraceEvent(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id, It.Is(format), It.Is(args)
                ))
                .Verifiable();
        }

        protected void ExpectTraceData(TraceEventType type, int id, object data)
        {
            Listener
                .Setup(t => t.TraceData(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id, data
                ))
                .Verifiable();
        }

        protected void ExpectTraceData(TraceEventType type, int id, params object[] data)
        {
            Listener
                .Setup(t => t.TraceData(
                    It.IsNotNull<TraceEventCache>(), TraceName, type, id, data
                ))
                .Verifiable();
        }

        protected void ExpectTraceTransfer(int id, string message, Guid activityId)
        {
            Listener
                .Setup(t => t.TraceTransfer(
                    It.IsNotNull<TraceEventCache>(), TraceName, id, message, activityId
                ))
                .Verifiable();
        }

        protected void ExpectTraceOperation(string name)
        {
            if (IsStaticTrace)
                ExpectTraceEvent(Information, 0, name + ": Starting");
            else
                ExpectTraceEvent(Information, 0, name + ": Starting", null);

            ExpectTraceEvent(Information, 0,
                s => s.Contains(": Completed"), // ordinal
                a => a.OfType<string>().Contains(name, StringComparer.Ordinal)
            );
        }

        protected static Exception CreateTestException()
            => new ApplicationException("Something bad happened.");
    }
}
