namespace AozoraSharp.Logging;

public sealed class LogManager
{
    public static LogManager Instance { get; } = new();

    private LoggerCreator loggerCreator = tag => new FileLogger(tag);

    public void SetLoggerCreator(LoggerCreator creator)
    {
        loggerCreator = creator;
    }
    public ILogger GetLogger(string tag)
    {
        return loggerCreator.Invoke(tag);
    }

    public delegate ILogger LoggerCreator(string tag);
}
