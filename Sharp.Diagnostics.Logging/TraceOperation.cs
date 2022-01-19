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

#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sharp.Diagnostics.Logging
{
    /// <summary>
    ///   Represents an operation whose start and end are logged to a trace
    ///   source.
    /// </summary>
    public sealed class TraceOperation : IDisposable
    {
        private readonly TraceSource? _trace;
        private readonly Activity     _activity;

        /// <summary>
        ///   Initializes a new <see cref="TraceOperation"/> instance with the
        ///   specified operation name.  The instance will use the static
        ///   <see cref="Trace"/> class to log the start and end of the
        ///   operation.
        /// </summary>
        /// <param name="name">
        ///   The name of the operation.  The default value is the member name
        ///   of the calling method or property, if the compiler supports
        ///   <c>CallerMemberNameAttribute</c>, and <c>null</c> otherwise.
        /// </param>
        public TraceOperation([CallerMemberName] string? name = null)
            : this(null, name) { }

        /// <summary>
        ///   Initializes a new <see cref="TraceOperation"/> instance with the
        ///   specified trace source and operation name.
        /// </summary>
        /// <param name="trace">
        ///   The trace source used to log the start and end of the operation.
        /// </param>
        /// <param name="name">
        ///   The name of the operation.  The default value is the member name
        ///   of the calling method or property, if the compiler supports
        ///   <c>CallerMemberNameAttribute</c>, and <c>null</c> otherwise.
        /// </param>
        public TraceOperation(TraceSource? trace, [CallerMemberName] string? name = null)
        {
            _trace = trace;

            // Will throw ArgumentException if name is null or empty
            _activity = new Activity(name!).Start();

            StartCorrelationManagerOperation(_activity);

            TraceStarting();
        }

        void IDisposable.Dispose()
        {
            _activity.Stop();

            TraceCompleted();

            StopCorrelationManagerOperation();
        }

        /// <summary>
        ///   Gets the name of the operation.
        /// </summary>
        public string? Name => _activity.OperationName;

        /// <summary>
        ///   Gets the UTC time when the operation started.
        /// </summary>
        public DateTime StartTime => _activity.StartTimeUtc;

        /// <summary>
        ///   Gets the duration elapsed since the operation started.
        /// </summary>
        public TimeSpan ElapsedTime
            => _activity.Duration > TimeSpan.Zero
                ? _activity.Duration
                : GetUtcNow() - _activity.StartTimeUtc;

        /// <summary>
        ///   Gets or sets the exception associated with the operation.
        /// </summary>
        public Exception? Exception { get; set; }

        private static void StartCorrelationManagerOperation(Activity activity)
        {
            if (!TryGetTraceId(activity, out var traceId))
                traceId = Guid.NewGuid();

            if (Trace.CorrelationManager.LogicalOperationStack.Count == 0)
                Trace.CorrelationManager.ActivityId = traceId;

            Trace.CorrelationManager.StartLogicalOperation(activity.Id);
        }

        private static void StopCorrelationManagerOperation()
        {
            Trace.CorrelationManager.StopLogicalOperation();

            if (Trace.CorrelationManager.LogicalOperationStack.Count == 0)
                Trace.CorrelationManager.ActivityId = Guid.Empty;
        }

        private static bool TryGetTraceId(Activity activity, out Guid traceId)
        {
            const string HexDigitsFormat = "N";

            if (activity.IdFormat != ActivityIdFormat.W3C)
            {
                traceId = default;
                return false;
            }

            return Guid.TryParseExact(
                activity.TraceId.ToString(), HexDigitsFormat, out traceId
            );
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries
        ///   to <see cref="Trace"/>.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void Do(string? name, Action action)
        {
            Do(null, name, action);
        }

        /// <summary>
        ///   Runs a logical operation asynchronously, writing start, stop, and
        ///   error entries to <see cref="Trace"/>.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static Task DoAsync(string? name, Func<Task> action)
        {
            return DoAsync(null, name, action);
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries
        ///   to the specified trace source.
        /// </summary>
        /// <param name="trace">The trace source to which to write.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static void Do(TraceSource? trace, string? name, Action action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            using var operation = new TraceOperation(trace, name);

            try
            {
                action();
            }
            catch (Exception e)
            {
                operation.Exception = e;
                throw;
            }
        }

        /// <summary>
        ///   Runs a logical operation asynchronously, writing start, stop, and
        ///   error entries to the specified trace source.
        /// </summary>
        /// <param name="trace">The trace source to which to write.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task DoAsync(TraceSource? trace, string? name, Func<Task> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            using var operation = new TraceOperation(trace, name);

            try
            {
                await action();
            }
            catch (Exception e)
            {
                operation.Exception = e;
                throw;
            }
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries
        ///   to <see cref="Trace"/>.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static TResult Do<TResult>(string? name, Func<TResult> action)
        {
            return Do(null, name, action);
        }

        /// <summary>
        ///   Runs a logical operation asynchronously, writing start, stop, and
        ///   error entries to <see cref="Trace"/>.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static Task<TResult> DoAsync<TResult>(string? name, Func<Task<TResult>> action)
        {
            return DoAsync(null, name, action);
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries
        ///   to the specified trace source.
        /// </summary>
        /// <param name="trace">The trace source to which to write.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static TResult Do<TResult>(TraceSource? trace, string? name, Func<TResult> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            using var operation = new TraceOperation(trace, name);

            try
            {
                return action();
            }
            catch (Exception e)
            {
                operation.Exception = e;
                throw;
            }
        }

        /// <summary>
        ///   Runs a logical operation asynchronously, writing start, stop, and
        ///   error entries to the specified trace source.
        /// </summary>
        /// <param name="trace">The trace source to which to write.</param>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="action"/> is <c>null</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task<TResult> DoAsync<TResult>(TraceSource? trace, string? name, Func<Task<TResult>> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            using var operation = new TraceOperation(trace, name);

            try
            {
                return await action();
            }
            catch (Exception e)
            {
                operation.Exception = e;
                throw;
            }
        }

        private void TraceStarting()
        {
            var name = Name;
            if (name == null)
                return;

            var trace = _trace;
            if (trace != null)
                trace.TraceInformation(name + ": Starting");
            else
                Trace.TraceInformation(name + ": Starting");
        }

        private void TraceCompleted()
        {
            var name = Name;
            if (name == null)
                return;

            var time      = ElapsedTime.TotalSeconds;
            var exception = Exception;
            var trace     = _trace;
            string notice;

            if (exception != null)
            {
                if (trace != null)
                    trace.TraceData(TraceEventType.Error, 0, exception);
                else
                    // The legacy Trace API does not have a TraceData method.
                    Trace.TraceError(exception.ToString());

                notice = " [EXCEPTION]";
            }
            else
            {
                notice = "";
            }

            if (trace != null)
                trace.TraceInformation("{0}: Completed [{1:N3}s]{2}", name, time, notice);
            else
                Trace.TraceInformation("{0}: Completed [{1:N3}s]{2}", name, time, notice);
        }

#if NETFRAMEWORK
        // HACK: Activity uses some internal magic to get better accuracy than
        //       DateTime.UtcNow normally delivers.  To have reliable date math
        //       against Activity, this class must use that same logic.
        internal static DateTime GetUtcNow()
        {
            var a = new Activity(nameof(GetUtcNow));
            a.Start().Stop();
            return a.StartTimeUtc;
        }
#else
        internal static DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
#endif
    }
}
