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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sharp.Diagnostics.Logging
{
    /// <summary>
    ///   Convenience methods for <c>TraceSouce</c>-based logging.
    /// </summary>
    public static class TraceSourceExtensions
    {
        #region Critical

        /// <summary>
        ///   Writes a critical error event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceCritical(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Critical, 0, message);

        /// <summary>
        ///   Writes a critical error event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceCritical(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Critical, id, message);

        /// <summary>
        ///   Writes a critical error event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceCritical(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Critical, 0, format, args);

        /// <summary>
        ///   Writes a critical error event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceCritical(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Critical, id, format, args);

        /// <summary>
        ///   Writes a critical error event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceCritical(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Critical, 0, exception);

        /// <summary>
        ///   Writes a critical error event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceCritical(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Critical, id, exception);

        #endregion
        #region Error

        /// <summary>
        ///   Writes an error event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceError(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Error, 0, message);

        /// <summary>
        ///   Writes an error event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceError(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Error, id, message);

        /// <summary>
        ///   Writes an error event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceError(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Error, 0, format, args);

        /// <summary>
        ///   Writes an error event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceError(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Error, id, format, args);

        /// <summary>
        ///   Writes an error event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceError(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Error, 0, exception);

        /// <summary>
        ///   Writes an error event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceError(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Error, id, exception);

        #endregion
        #region Warning

        /// <summary>
        ///   Writes a warning event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceWarning(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Warning, 0, message);

        /// <summary>
        ///   Writes a warning event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceWarning(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Warning, id, message);

        /// <summary>
        ///   Writes a warning event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceWarning(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Warning, 0, format, args);

        /// <summary>
        ///   Writes a warning event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceWarning(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Warning, id, format, args);

        /// <summary>
        ///   Writes a warning event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceWarning(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Warning, 0, exception);

        /// <summary>
        ///   Writes a warning event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceWarning(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Warning, id, exception);

        #endregion
        #region Information

        // NOTE: Provided by TraceSource:
        // void TraceInformation(string message)

        /// <summary>
        ///   Writes an informational event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceInformation(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Information, id, message);

        // NOTE: Provided by TraceSource:
        // void TraceInformation(string format, params object[] args)

        /// <summary>
        ///   Writes an informational event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceInformation(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Information, id, format, args);

        /// <summary>
        ///   Writes an informational event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceInformation(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Information, 0, exception);

        /// <summary>
        ///   Writes an informational event to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceInformation(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Information, id, exception);

        #endregion
        #region Verbose

        /// <summary>
        ///   Writes a verbose event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceVerbose(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Verbose, 0, message);

        /// <summary>
        ///   Writes a verbose event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceVerbose(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Verbose, id, message);

        /// <summary>
        ///   Writes a verbose event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceVerbose(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Verbose, 0, format, args);

        /// <summary>
        ///   Writes a verbose event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceVerbose(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Verbose, id, format, args);

        /// <summary>
        ///   Writes a verbose event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceVerbose(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Verbose, 0, exception);

        /// <summary>
        ///   Writes a verbose event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceVerbose(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Verbose, id, exception);

        #endregion
        #region Start

        /// <summary>
        ///   Writes a start event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceStart(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Start, 0, message);

        /// <summary>
        ///   Writes a start event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceStart(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Start, id, message);

        /// <summary>
        ///   Writes a start event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceStart(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Start, 0, format, args);

        /// <summary>
        ///   Writes a start event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceStart(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Start, id, format, args);

        /// <summary>
        ///   Writes a start event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceStart(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Start, 0, exception);

        /// <summary>
        ///   Writes a start event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceStart(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Start, id, exception);

        #endregion
        #region Stop

        /// <summary>
        ///   Writes a stop event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceStop(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Stop, 0, message);

        /// <summary>
        ///   Writes a stop event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceStop(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Stop, id, message);

        /// <summary>
        ///   Writes a stop event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceStop(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Stop, 0, format, args);

        /// <summary>
        ///   Writes a stop event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceStop(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Stop, id, format, args);

        /// <summary>
        ///   Writes a stop event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceStop(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Stop, 0, exception);

        /// <summary>
        ///   Writes a stop event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceStop(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Stop, id, exception);

        #endregion
        #region Suspend

        /// <summary>
        ///   Writes a suspend event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceSuspend(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Suspend, 0, message);

        /// <summary>
        ///   Writes a suspend event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceSuspend(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Suspend, id, message);

        /// <summary>
        ///   Writes a suspend event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceSuspend(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Suspend, 0, format, args);

        /// <summary>
        ///   Writes a suspend event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceSuspend(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Suspend, id, format, args);

        /// <summary>
        ///   Writes a suspend event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceSuspend(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Suspend, 0, exception);

        /// <summary>
        ///   Writes a suspend event to the trace listeners in the <c>Listeners</c> collection
        ///   using the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceSuspend(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Suspend, id, exception);

        #endregion
        #region Resume

        /// <summary>
        ///   Writes a resume event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceResume(this TraceSource trace, string message)
            => trace.TraceEvent(TraceEventType.Resume, 0, message);

        /// <summary>
        ///   Writes a resume event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceResume(this TraceSource trace, int id, string message)
            => trace.TraceEvent(TraceEventType.Resume, id, message);

        /// <summary>
        ///   Writes a resume event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified format string and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceResume(this TraceSource trace, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Resume, 0, format, args);

        /// <summary>
        ///   Writes a resume event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceResume(this TraceSource trace, int id, string format, params object[] args)
            => trace.TraceEvent(TraceEventType.Resume, id, format, args);

        /// <summary>
        ///   Writes a resume event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceResume(this TraceSource trace, Exception exception)
            => trace.TraceData(TraceEventType.Resume, 0, exception);

        /// <summary>
        ///   Writes a resume event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event identifier and exception.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="exception">An exception to report in the event.</param>
        [Conditional("TRACE")]
        public static void TraceResume(this TraceSource trace, int id, Exception exception)
            => trace.TraceData(TraceEventType.Resume, id, exception);

        #endregion
        #region Operations / Correlation

        /// <summary>
        ///   Starts a logical operation, writing a start event to the trace listeners in the
        ///   <c>Listeners</c> collection.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="name">The name of the operation.</param>
        /// <returns>
        ///   An <c>TraceOperation</c> representing the logical operation.
        ///   When disposed, the <c>TraceOperation</c> writes stop and error entries to the trace listeners in the <c>Listeners</c> collection using the specified .
        /// </returns>
        public static TraceOperation Operation(this TraceSource trace, [CallerMemberName] string name = null)
        {
            return new TraceOperation(trace, name);
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries to the trace
        ///   listeners in the <c>Listeners</c> collection.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        [DebuggerStepThrough]
        public static void Do(this TraceSource trace, string name, Action action)
        {
            TraceOperation.Do(trace, name, action);
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries to the trace
        ///   listeners in the <c>Listeners</c> collection.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <returns>
        ///   The value returned by <paramref name="action"/>.
        /// </returns>
        [DebuggerStepThrough]
        public static T Do<T>(this TraceSource trace, string name, Func<T> action)
        {
            return TraceOperation.Do(trace, name, action);
        }

        // TODO: DoAsync

        // Provided by TraceSource:
        // void TraceTransfer(int id, string message, Guid newActivityId)

        /// <summary>
        ///   Writes a trace transfer message to the trace listeners in the <c>Listeners</c>
        ///   collection using the specified message and related activity identifier.
        /// </summary>
        /// <param name="trace">The trace source to which the message should be written.</param>
        /// <param name="message">The trace message to write.</param>
        /// <param name="relatedActivityId">The identifier of the related activity.</param>
        /// <remarks>
        ///   <c>TraceTransfer</c> is intended to be used with the logical activities of a
        ///   <c>CorrelationManager</c>. The <paramref name="relatedActivityId"/> parameter
        ///   relates to the <c>ActivityId</c> property of a <c>CorrelationManager</c> object.
        ///   If a logical operation begins in one activity and transfers to another,
        ///   the second activity should log the transfer by calling the <c>TraceTransfer</c>
        ///   method.  The <c>TraceTransfer</c> call relates the new activity identifier to the
        ///   previous one.  An example consumer of this functionality is a trace viewer that can
        ///   report logical operations that span multiple activities.
        /// </remarks>
        [Conditional("TRACE")]
        public static void TraceTransfer(this TraceSource trace, string message, Guid relatedActivityId)
            => trace.TraceTransfer(0, message, relatedActivityId);

        #endregion
        #region Event

        /// <summary>
        ///   Writes a trace event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event type and message.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="eventType">The type of event to write.</param>
        /// <param name="message">A message for the event.</param>
        [Conditional("TRACE")]
        public static void TraceEvent(this TraceSource trace, TraceEventType eventType, string message)
            => trace.TraceEvent(eventType, 0, message);

        // Provided by TraceSource:
        // void TraceEvent(TraceEventType eventType, int id, string message);

        /// <summary>
        ///   Writes a trace event to the trace listeners in the <c>Listeners</c> collection using
        ///   the specified event type, format string, and argument array.
        /// </summary>
        /// <param name="trace">The trace source to which the event should be written.</param>
        /// <param name="eventType">The type of event to write.</param>
        /// <param name="format">A format string to build a message for the event.</param>
        /// <param name="args">The objects to substitute into the format string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="format"/> is null.
        /// </exception>
        /// <exception cref="T:System.FormatException">
        ///   <paramref name="format"/> is invalid or
        ///   specifies an argument position not present in <paramref name="args"/>.
        /// </exception>
        [Conditional("TRACE")]
        public static void TraceEvent(this TraceSource trace, TraceEventType eventType, string format, params object[] args)
            => trace.TraceEvent(eventType, 0, format, args);

        // Provided by TraceSource:
        // void TraceEvent(TraceEventType eventType, int id, string format, params object[] args)

        // NOTE: Overloads for Exception not provided here.  Use TraceData() for exceptions.

        #endregion
        #region Data

        /// <summary>
        ///   Writes trace data to the trace listeners in the <c>Listeners</c> collection using the
        ///   specified event type and trace data.
        /// </summary>
        /// <param name="trace">The trace source to which the trace data should be written.</param>
        /// <param name="eventType">The event type of the trace data.</param>
        /// <param name="data">The trace data.</param>
        [Conditional("TRACE")]
        public static void TraceData(this TraceSource trace, TraceEventType eventType, object data)
            => trace.TraceData(eventType, 0, data);

        // Provided by TraceSource:
        // void TraceData(TraceEventType eventType, int id, object data)

        /// <summary>
        ///   Writes trace data to the trace listeners in the <c>Listeners</c> collection using the
        ///   specified event type and trace data array.
        /// </summary>
        /// <param name="trace">The trace source to which the trace data should be written.</param>
        /// <param name="eventType">The event type of the trace data.</param>
        /// <param name="data">An object array containing the trace data.</param>
        [Conditional("TRACE")]
        public static void TraceData(this TraceSource trace, TraceEventType eventType, params object[] data)
            => trace.TraceData(eventType, 0, data);

        // Provided by TraceSource:
        // void TraceData(TraceEventType eventType, int id, params object[] data)

        #endregion
    }
}
