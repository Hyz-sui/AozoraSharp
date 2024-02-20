using System;

namespace AozoraSharp.Logging;

public class ConsoleLogListener : ILogListener
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}
