namespace AozoraSharp.Logging;

public sealed class LogManager
{
    public static LogManager Instance { get; } = new();

    public ILogger GlobalLogger { get; private set; }
    public LogManager()
    {
        GlobalLogger = GetLogger("Global");
    }

    private LoggerCreator loggerCreator = tag => new FileLogger(tag);

    public void SetLoggerCreator(LoggerCreator creator)
    {
        loggerCreator = creator;
        GlobalLogger = GetLogger("Global");
    }
    public ILogger GetLogger(string tag)
    {
        return loggerCreator.Invoke(tag);
    }

    public delegate ILogger LoggerCreator(string tag);
}
