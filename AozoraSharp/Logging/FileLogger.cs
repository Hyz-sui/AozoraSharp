namespace AozoraSharp.Logging;

/// <summary>
/// Logger that writes to a log file.
/// </summary>
public class FileLogger(string tag) : Logger<FileLogListener>(tag)
{ }
