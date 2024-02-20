namespace AozoraSharp.Logging;

/// <summary>
/// Provides logging functions.
/// </summary>
public interface ILogger
{
    protected string Tag { get; }

    public void Info(string message);
    public void Warn(string message);
    public void Error(string message);
    public void Fatal(string message);
    public void Debug(string message);
    public void Write(string message);
}
