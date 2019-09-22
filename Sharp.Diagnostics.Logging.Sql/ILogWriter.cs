using System;

namespace Sharp.Diagnostics.Logging.Sql
{
    /// <summary>
    ///   Interface for types that provide a write-only, flushable queue of
    ///   <see cref="LogEntry"/> objects.
    /// </summary>
    public interface ILogWriter : IDisposable
    {
        /// <summary>
        ///   Enqueues the specified entry.
        /// </summary>
        /// <param name="entry">
        ///   The entry to enqueue.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="entry"/> is <c>null</c>.
        /// </exception>
        void Enqueue(LogEntry entry);

        /// <summary>
        ///   Flushes enqueued entries, causing them to be consumed from the
        ///   queue and written to a data store.
        /// </summary>
        void Flush();
    }
}