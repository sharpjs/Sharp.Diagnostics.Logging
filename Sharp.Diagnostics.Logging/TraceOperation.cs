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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable

namespace Sharp.Diagnostics.Logging
{
    /// <summary>
    ///   Represents an operation whose start and end are logged to a trace
    ///   source.
    /// </summary>
    public sealed class TraceOperation : IDisposable
    {
        private readonly TraceSource? _trace;
        private readonly string?      _name;
        private readonly DateTime     _start;
        private Exception?            _exception;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TraceOperation"/>
        ///   class with the specified operation name.
        /// </summary>
        /// <param name="name">
        ///   The name of the operation.  The default value is the member name
        ///   of the calling method or property, if the compiler supports
        ///   <c>CallerMemberNameAttribute</c>, and <c>null</c> otherwise.
        /// </param>
        public TraceOperation([CallerMemberName] string? name = null)
            : this(null, name) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TraceOperation"/>
        ///   class with the specified trace source and operation name.
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
            _name  = name;

            if (Trace.CorrelationManager.LogicalOperationStack.Count == 0)
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();

            Trace.CorrelationManager.StartLogicalOperation();

            TraceStarting();

            _start = DateTime.UtcNow;
        }

        private void Dispose()
        {
            TraceCompleted();

            Trace.CorrelationManager.StopLogicalOperation();

            if (Trace.CorrelationManager.LogicalOperationStack.Count == 0)
                Trace.CorrelationManager.ActivityId = Guid.Empty;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        /// <summary>
        ///   Gets the name of the operation.
        /// </summary>
        public string? Name
        {
            get { return _name; }
        }

        /// <summary>
        ///   Gets the UTC time when the operation started.
        /// </summary>
        public DateTime StartTime
        {
            get { return _start; }
        }

        /// <summary>
        ///   Gets the duration elapsed since the operation started.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return DateTime.UtcNow - _start; }
        }

        /// <summary>
        ///   Gets or sets the exception associated with the operation.
        /// </summary>
        public Exception? Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        /// <summary>
        ///   Runs a logical operation, writing start, stop, and error entries
        ///   to <see cref="Trace"/>.
        /// </summary>
        /// <param name="name">The name of the operation.</param>
        /// <param name="action">The operation.</param>
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
            if (_name == null)
                return;

            var trace = _trace;
            if (trace != null)
                trace.TraceInformation(_name + ": Starting");
            else
                Trace.TraceInformation(_name + ": Starting");
        }

        private void TraceCompleted()
        {
            if (_name == null)
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
                trace.TraceInformation("{0}: Completed [{1:N3}s]{2}", _name, time, notice);
            else
                Trace.TraceInformation("{0}: Completed [{1:N3}s]{2}", _name, time, notice);
        }
    }
}
