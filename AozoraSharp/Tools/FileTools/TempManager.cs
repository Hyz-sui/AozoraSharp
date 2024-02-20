using System.IO;

namespace AozoraSharp.Tools.FileTools;

public sealed class TempManager
{
    public static TempManager Instance { get; } = new();

    public TempManager()
    {
        Initialize();
    }

    private readonly DirectoryInfo tempDir = new DirectoryInfo($"{Path.GetTempPath()}/AozoraSharp/");

    /// <summary>
    /// Cleanup temporary files.
    /// </summary>
    private void Initialize()
    {
        if (tempDir.Exists)
        {
            tempDir.Delete(true);
        }
        tempDir.Create();
    }
    /// <summary>
    /// Create a temporary file.
    /// </summary>
    /// <param name="extension">extension</param>
    /// <returns>created temporary file</returns>
    public TempFile CreateTempFile(string extension = null)
    {
        var name = Path.GetRandomFileName();
        if (extension != null)
        {
            name = $"{name}.{extension}";
        }
        var path = Path.Combine(tempDir.FullName, name);
        var file = new TempFile(path);
        return file;
    }
}
