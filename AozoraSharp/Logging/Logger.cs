using System;

namespace AozoraSharp.Logging;

public abstract class Logger<T>(string tag) : ILogger where T : ILogListener, new()
{
    public string Tag { get; } = tag;

    private static readonly T Listener = new();

    public virtual void Debug(string message)
    {
#if DEBUG
        Write($"DEBUG {message}");
#endif
    }

    public virtual void Error(string message)
    {
        Write($"ERROR {message}");
    }

    public virtual void Fatal(string message)
    {
        Write($"FATAL {message}");
    }

    public virtual void Info(string message)
    {
        Write($"INFO  {message}");
    }

    public virtual void Warn(string message)
    {
        Write($"WARN  {message}");
    }

    public virtual void Write(string message)
    {
        Listener.Write($"AozoraSharp   {Tag,-17}{DateTime.UtcNow:HH:mm:ss.ffff}   {message}");
    }
}
