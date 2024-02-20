using System.IO;

namespace AozoraSharp.Tools.FileTools;

/// <summary>
/// Represents a temporary file.
/// </summary>
public sealed class TempFile(string path)
{
    public FileInfo File { get; } = new FileInfo(path);

    public string Path => File.FullName;
}
