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
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using static System.Diagnostics.TraceEventType;

namespace Sharp.Diagnostics.Logging
{
    [TestFixture]
    public class PrettyTextWriterTraceListenerTests
    {
        const string Source = "Test";

        private string          FileName;
        private TraceEventCache Cache;

        [SetUp]
        public void SetUp()
        {
            FileName = Path.GetTempFileName();
            Cache    = new TraceEventCache();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(FileName);
        }

        [Test]
        public void Construct_Default()
        {
            using (var listener = new PrettyTextWriterTraceListener())
                listener.Writer.Should().BeSameAs(Console.Out);
        }

        [Test]
        public void Construct_FileName()
        {
            using (var listener = new PrettyTextWriterTraceListener(FileName))
                listener.Writer.Should().NotBeNull().And.NotBeSameAs(Console.Out);
        }

        [Test]
        public void TraceEvent_Id_Unfiltered()
        {
            Test(
                listener =>
                {
                    listener.TraceEvent(Cache, Source, Verbose, 1);
                },
                expected: Line(Verbose, 1, "")
            );
        }

        [Test]
        public void TraceEvent_Id_FilteredIn()
        {
            Test(
                listener =>
                {
                    listener.Filter = Filter(Verbose, 1, "", ok: true);
                    listener.TraceEvent(Cache, Source, Verbose, 1);
                },
                expected: Line(Verbose, 1, "")
            );
        }

        [Test]
        public void TraceEvent_Id_FilteredOut()
        {
            Test(
                listener =>
                {
                    listener.Filter = Filter(Verbose, 1, "", ok: false);
                    listener.TraceEvent(Cache, Source, Verbose, 1);
                },
                expected: ""
            );
        }

        [Test]
        public void TraceEvent_Message_Unfiltered()
        {
            Test(
                listener =>
                {
                    listener.TraceEvent(Cache, Source, Error, 2, "Fthurp");
                },
                expected: Line(Error, 2, "Fthurp")
            );
        }

        [Test]
        public void TraceEvent_Message_FilteredIn()
        {
            Test(
                listener =>
                {
                    listener.Filter = Filter(Error, 2, "Fthurp", ok: true);
                    listener.TraceEvent(Cache, Source, Error, 2, "Fthurp");
                },
                expected: Line(Error, 2, "Fthurp")
            );
        }

        [Test]
        public void TraceEvent_Message_FilteredOut()
        {
            Test(
                listener =>
                {
                    listener.Filter = Filter(Error, 2, "Fthurp", ok: false);
                    listener.TraceEvent(Cache, Source, Error, 2, "Fthurp");
                },
                expected: ""
            );
        }

        [Test]
        public void TraceEvent_Format_Unfiltered()
        {
            var args = new object[] { 1234 };

            Test(
                listener =>
                {
                    listener.TraceEvent(Cache, Source, Warning, 3, "Fnarg {0:X}", args);
                },
                expected: Line(Warning, 3, "Fnarg 4D2")
            );
        }

        [Test]
        public void TraceEvent_Format_FilteredIn()
        {
            var args = new object[] { 1234 };

            Test(
                listener =>
                {
                    listener.Filter = Filter(Warning, 3, "Fnarg {0:X}", args, ok: true);
                    listener.TraceEvent(Cache, Source, Warning, 3, "Fnarg {0:X}", args);
                },
                expected: Line(Warning, 3, "Fnarg 4D2")
            );
        }

        [Test]
        public void TraceEvent_Format_FilteredOut()
        {
            var args = new object[] { 1234 };

            Test(
                listener =>
                {
                    listener.Filter = Filter(Warning, 3, "Fnarg {0:X}", args, ok: false);
                    listener.TraceEvent(Cache, Source, Warning, 3, "Fnarg {0:X}", args);
                },
                expected: ""
            );
        }

        [Test]
        public void TraceEvent_Format_NullArgs()
        {
            var args = new object[] { 1234 };

            Test(
                listener =>
                {
                    listener.TraceEvent(Cache, Source, Warning, 3, "Fnarg", null as object[]);
                },
                expected: Line(Warning, 3, "Fnarg")
            );
        }

        [Test]
        public void TraceData_Object_Unfiltered()
        {
            var obj = "ObjectA";

            Test(
                listener =>
                {
                    listener.TraceData(Cache, Source, Information, 4, obj);
                },
                expected: Line(Information, 4, "ObjectA")
            );
        }

        [Test]
        public void TraceData_Object_FilteredIn()
        {
            var obj = "ObjectA";

            Test(
                listener =>
                {
                    listener.Filter = Filter(Information, 4, obj: obj, ok: true);
                    listener.TraceData(Cache, Source, Information, 4, obj);
                },
                expected: Line(Information, 4, "ObjectA")
            );
        }

        [Test]
        public void TraceData_Object_FilteredOut()
        {
            var obj = "ObjectA";

            Test(
                listener =>
                {
                    listener.Filter = Filter(Information, 4, obj: obj, ok: false);
                    listener.TraceData(Cache, Source, Information, 4, obj);
                },
                expected: ""
            );
        }

        [Test]
        public void TraceData_Object_Null()
        {
            Test(
                listener =>
                {
                    listener.TraceData(Cache, Source, Information, 5, null as object);
                },
                expected: Line(Information, 5, "(null)")
            );
        }

        [Test]
        public void TraceData_Array_Unfiltered()
        {
            var objs = new object[] { "ObjectA", "ObjectB" };

            Test(
                listener =>
                {
                    listener.TraceData(Cache, Source, Information, 6, objs);
                },
                expected: Line(Information, 6, "ObjectA, ObjectB")
            );
        }

        [Test]
        public void TraceData_Array_FilteredIn()
        {
            var objs = new object[] { "ObjectA", "ObjectB" };

            Test(
                listener =>
                {
                    listener.Filter = Filter(Information, 6, objs: objs, ok: true);
                    listener.TraceData(Cache, Source, Information, 6, objs);
                },
                expected: Line(Information, 6, "ObjectA, ObjectB")
            );
        }

        [Test]
        public void TraceData_Array_FilteredOut()
        {
            var objs = new object[] { "ObjectA", "ObjectB" };

            Test(
                listener =>
                {
                    listener.Filter = Filter(Information, 6, objs: objs, ok: false);
                    listener.TraceData(Cache, Source, Information, 6, objs);
                },
                expected: ""
            );
        }

        [Test]
        public void TraceData_Array_NullElement()
        {
            Test(
                listener =>
                {
                    listener.TraceData(Cache, Source, Information, 7, null, null);
                },
                expected: Line(Information, 7, "(null), (null)")
            );
        }

        [Test]
        public void TraceData_Array_NullArray()
        {
            Test(
                listener =>
                {
                    listener.TraceData(Cache, Source, Information, 8, null as object[]);
                },
                expected: Line(Information, 8, "")
            );
        }

        [Test]
        public void TraceTransfer_Unfiltered()
        {
            var relatedId = Guid.NewGuid();

            Test(
                listener =>
                {
                    listener.TraceTransfer(Cache, Source, 9, "Shklep", relatedId);
                },
                expected: Line(Transfer, 9, $"Shklep {{related:{relatedId}}}")
            );
        }

        [Test]
        public void TraceTransfer_FilteredIn()
        {
            var relatedId = Guid.NewGuid();
            var args      = new object[] { "Shklep", relatedId };

            Test(
                listener =>
                {
                    listener.Filter = Filter(Transfer, 9, "{0} {{related:{1}}}", args, ok: true);
                    listener.TraceTransfer(Cache, Source, 9, "Shklep", relatedId);
                },
                expected: Line(Transfer, 9, $"Shklep {{related:{relatedId}}}")
            );
        }

        [Test]
        public void TraceTransfer_FilteredOut()
        {
            var relatedId = Guid.NewGuid();
            var args      = new object[] { "Shklep", relatedId };

            Test(
                listener =>
                {
                    listener.Filter = Filter(Transfer, 9, "{0} {{related:{1}}}", args, ok: false);
                    listener.TraceTransfer(Cache, Source, 9, "Shklep", relatedId);
                },
                expected: ""
            );
        }

        [Test]
        public void WriteLine()
        {
            Test(l => l.WriteLine("Yoist"), "Yoist" + Environment.NewLine);
        }

        [Test]
        public void Write()
        {
            Test(l => l.Write("Turgle"), "Turgle");
        }

        [Test]
        public void Write_ThenTraceEvent()
        {
            Test(
                listener =>
                {
                    listener.Write("Turgle");
                    listener.TraceEvent(Cache, Source, Error, 2, "Flimp");
                },
                expected: string.Concat(
                    "Turgle",
                    Environment.NewLine,
                    Line(Error, 2, "Flimp")
                )
            );
        }

        private void Test(Action<PrettyTextWriterTraceListener> action, string expected)
        {
            using (var listener = new PrettyTextWriterTraceListener(FileName))
                action(listener);

            var content = File.ReadAllText(FileName, Encoding.UTF8);

            content.Should().Be(expected);
        }

        private TraceFilter Filter(
            TraceEventType type,
            int            id,
            string         message     = null,
            object[]       args        = null,
            object         obj         = null,
            object[]       objs        = null,
            bool           ok = false)
        {
            var filter = new Mock<TraceFilter>(MockBehavior.Strict);

            filter
                .Setup(f => f.ShouldTrace(Cache, Source, type, id, message, args, obj, objs))
                .Returns(ok);

            return filter.Object;
        }

        private string Line(TraceEventType type, int id, string message)
        {
            return string.Format(
                "[{0:yyyy-MM-dd HH:mm:ss.fff}] ({1}:{2}) {3}:  #{4}: {5} <{6}>{7}",
                Cache.DateTime,
                Cache.ProcessId,
                Cache.ThreadId,
                type,
                id,
                message,
                Source,
                Environment.NewLine
            );
        }
    }
}
