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

using System.Text;

namespace Sharp.Diagnostics.Logging;

/// <summary>
///   Custom trace listener that writes to a log file in a concise format.
/// </summary>
public class PrettyTextWriterTraceListener : TextWriterTraceListener
{
    private const string
        DefaultLogFileName = "Log.txt";

    /// <summary>
    ///   Initializes a new <see cref="PrettyTextWriterTraceListener"/>
    ///   instance that writes to the console standard output stream.
    /// </summary>
    public PrettyTextWriterTraceListener()
        : base(Console.Out) { }

    /// <summary>
    ///   Initializes a new <see cref="PrettyTextWriterTraceListener"/>
    ///   instance that writes to the specified file.
    /// </summary>
    /// <param name="fileName">
    ///   The path of the file to write, relative to the program.
    /// </param>
    public PrettyTextWriterTraceListener(string fileName)
        : this(fileName, new Shim()) { }

    // For testing
    internal PrettyTextWriterTraceListener(string fileName, IShim shim)
#if NETFRAMEWORK
        : base(GetFullPath(fileName, shim)) { }
#else
        // Workaround for .NET Core bug https://github.com/dotnet/corefx/issues/28747
        : base(CreateWriter(fileName, shim)) { }
#endif

    /// <inheritdoc/>
    public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id)
    {
        TraceEvent(e, source, type, id, string.Empty);
    }

    /// <inheritdoc/>
    public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id, string message)
    {
        if (!TryGetWriter(out var writer))
            return;

        if (!ShouldTrace(e, source, type, id, message))
            return;

        WriteHeader(e, type, id);
        writer.Write(message);
        WriteFooter(source);
    }

    /// <inheritdoc/>
    public override void TraceEvent(TraceEventCache e, string source, TraceEventType type, int id, string format, params object[] args)
    {
        if (!TryGetWriter(out var writer))
            return;

        if (!ShouldTrace(e, source, type, id, format, args))
            return;

        WriteHeader(e, type, id);
        if (args != null)
            writer.Write(string.Format(format, args));
        else
            writer.Write(format);
        WriteFooter(source);
    }

    /// <inheritdoc/>
    public override void TraceData(TraceEventCache e, string source, TraceEventType type, int id, object obj)
    {
        if (!TryGetWriter(out var writer))
            return;

        if (!ShouldTrace(e, source, type, id, obj: obj))
            return;

        WriteHeader(e, type, id);
        writer.Write(Format(obj));
        WriteFooter(source);
    }

    /// <inheritdoc/>
    public override void TraceData(TraceEventCache e, string source, TraceEventType type, int id, params object[] objs)
    {
        if (!TryGetWriter(out var writer))
            return;

        if (!ShouldTrace(e, source, type, id, objs: objs))
            return;

        WriteHeader(e, type, id);
        WriteObjects(objs);
        WriteFooter(source);
    }

    /// <inheritdoc/>
    public override void TraceTransfer(TraceEventCache e, string source, int id, string message, Guid relatedActivityId)
    {
        TraceEvent(e, source, TraceEventType.Transfer, id, "{0} {{related:{1}}}", message, relatedActivityId);
    }

    /// <inheritdoc/>
    public override void Write(string message)
    {
        if (!TryGetWriter(out var writer))
            return;

        writer.Write(message);
        NeedIndent = false;
    }

    /// <inheritdoc/>
    public override void WriteLine(string message)
    {
        if (!TryGetWriter(out var writer))
            return;

        writer.WriteLine(message);
        NeedIndent = true;
    }

    private void WriteHeader(TraceEventCache e, TraceEventType type, int id)
    {
        var writer = Writer;

        if (!NeedIndent)
            // Events always start on a new line.
            writer.WriteLine();

        // Date, Time
        writer.Write('[');
        writer.Write(e.DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", writer.FormatProvider));
        writer.Write(']');
        writer.Write(' ');

        // Process, Thread
        writer.Write('(');
        writer.Write(e.ProcessId);
        writer.Write(':');
        writer.Write(e.ThreadId);
        writer.Write(')');
        writer.Write(' ');

        // Correlation Id
        var operations = e.LogicalOperationStack;
        if (operations.Count > 0)
        {
            writer.Write('{');
            writer.Write(operations.Peek());
            writer.Write('}');
            writer.Write(' ');
        }

        // Event Type
        writer.Write(type.ToString());
        writer.Write(':');
        writer.Write(' ');

        // Event ID
        if (id != 0)
        {
            writer.Write(' ');
            writer.Write('#');
            writer.Write(id);
            writer.Write(':');
            writer.Write(' ');
        }

        NeedIndent = false;
    }

    private void WriteFooter(string source)
    {
        var writer = Writer;

        // Source
        writer.Write(' ');
        writer.Write('<');
        writer.Write(source);
        writer.Write('>');

        // EOL
        writer.WriteLine();
        NeedIndent = true;
    }

    private void WriteObjects(object[] objs)
    {
        var writer = Writer;

        if (objs != null && objs.Length != 0)
        {
            writer.Write(Format(objs[0]));

            for (var i = 1; i < objs.Length; i++)
            {
                writer.Write(", ");
                writer.Write(Format(objs[i]));
            }
        }

        NeedIndent = false;
    }

    /// <inheritdoc/>
    protected sealed override void WriteIndent()
    {
        // Do nothing
    }

    // For testing
    internal void InvokeWriteIndent()
    {
        WriteIndent();
    }

    private static string Format(object obj)
    {
        return obj == null ? "(null)" : obj.ToString();
    }

    private bool TryGetWriter(out TextWriter writer)
    {
        var instance = writer = Writer;
        return instance != null
            && instance != TextWriter.Null;
    }

    private bool ShouldTrace(TraceEventCache e, string source, TraceEventType type, int id,
        string? message = null, object[]? args = null, object? obj = null, object[]? objs = null)
    {
        var filter = Filter;
        return filter == null
            || filter.ShouldTrace(e, source, type, id, message, args, obj, objs);
    }

#if !NETFRAMEWORK
    // Workaround for .NET Core bug https://github.com/dotnet/corefx/issues/28747

    private static TextWriter CreateWriter(string? path, IShim shim)
    {
        path = GetFullPath(path, shim);
        if (path is null)
            // Disable the listener
            return TextWriter.Null;

        var name = Path.GetFileName(path);

        var encoding = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes:             false
        );

        for (var i = 0; i < 2; i++)
        {
            try
            {
                return shim.CreateStreamWriter(path, encoding);
            }
            catch (IOException)
            {
                // Retry using a fallback filename
                var fallbackName = Guid.NewGuid().ToString() + name;
                path = Path.GetDirectoryName(path);
                path = Path.Combine(path, fallbackName);
                continue;
            }
            catch
            {
                // Give up
                break;
            }
        }

        NotifyCannotCreateLogFile(path, shim);

        // Disable the listener
        return TextWriter.Null;
    }
#endif

    private static string? GetFullPath(string? fileName, IShim shim)
    {
        // Choose a reasonable default file name
        var path = string.IsNullOrEmpty(fileName)
            ? DefaultLogFileName
            : fileName!;

        try
        {
            // Convert a relative path to full
            if (!Path.IsPathRooted(path))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            // Ensure the directory exists and is writable
            EnsureDirectory(path);

            // The path *should* work at this point
            return path;
        }
        catch
        {
            // Use best effort to give the admin a helpful hint
            NotifyCannotCreateLogFile(path, shim);

            // Disable the listener
            return null;
        }
    }

    private static void EnsureDirectory(string path)
    {
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(path);
        Directory.CreateDirectory(directory);

        // Probe for writability by creating a random file
        var probe = Path.Combine(directory, Path.GetRandomFileName());
        using (File.Open(probe, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) { }
        File.Delete(probe);
    }

    private static void NotifyCannotCreateLogFile(string path, IShim shim)
    {
        try
        {
            shim.NotifyCriticalError($"Unable to create log file: {path}");
        }
        catch
        {
            // This code is best-effort only and should not overwrite
            // whatever exception is being processed already.
        }
    }

    internal interface IShim
    {
#if NETSTANDARD
        StreamWriter CreateStreamWriter(string path, Encoding encoding);
#endif
        void NotifyCriticalError(string message);
    }

#if NETFRAMEWORK
    private class Shim : IShim
    {
        public void NotifyCriticalError(string message)
        {
            using (var log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry(message, EventLogEntryType.Error);
            }
        }
    }
#else
    private class Shim : IShim
    {
        private const int BufferSize = 4096; // bytes

        public StreamWriter CreateStreamWriter(string path, Encoding encoding)
            => new StreamWriter(path, true /*append*/, encoding, BufferSize);

        public void NotifyCriticalError(string message)
            => Console.Error.WriteLine(message);
    }
#endif
}
