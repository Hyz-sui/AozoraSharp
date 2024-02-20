using System;
using System.IO;
using System.Text;
using System.Threading;

namespace AozoraSharp.Logging;

public class FileLogListener : IDisposable, ILogListener
{
    private const string LogFilePath = "./AozoraSharpOutput.log";
    private const string ExLogFilePath = "./AozoraSharpOutput_{0}.log";
    private int exCount = 0;
    private readonly TextWriter writer;
    private readonly Timer flusher;

    public FileLogListener()
    {
        try
        {
            writer = TextWriter.Synchronized(GetStreamWriter(LogFilePath));
        }
        catch (IOException)
        {
            writer = TextWriter.Synchronized(GetExStreamWriter());
        }
        flusher = new(_ => writer.Flush(), default, 1000, 1000);
    }
    private StreamWriter GetExStreamWriter()
    {
        try
        {
            return GetStreamWriter(string.Format(ExLogFilePath, exCount));
        }
        catch (IOException)
        {
            exCount++;
            return exCount >= 16 ? throw new Exception("Failed to open log file.") : GetExStreamWriter();
        }
    }
    private StreamWriter GetStreamWriter(string path)
    {
        var stream = new FileStream(new FileInfo(path).FullName, FileMode.Create, FileAccess.Write, FileShare.Read);
        var streamWriter = new StreamWriter(stream, Encoding.UTF8) { NewLine = "\n" };
        return streamWriter;
    }

    public void Write(string message)
    {
        writer.WriteLine(message);
    }

    private bool disposedValue;

    protected virtual void Dispose(bool isDisposing)
    {
        if (!disposedValue)
        {
            writer.Flush();
            if (isDisposing)
            {
                flusher.Dispose();
                writer.Dispose();
            }
            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(isDisposing: true);
        GC.SuppressFinalize(this);
    }
    ~FileLogListener()
    {
        Dispose(false);
    }
}
