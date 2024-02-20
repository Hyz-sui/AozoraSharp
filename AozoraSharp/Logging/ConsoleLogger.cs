namespace AozoraSharp.Logging;

/// <summary>
/// Logger with Console.WriteLine
/// </summary>
public class ConsoleLogger(string tag) : Logger<ConsoleLogListener>(tag)
{ }
