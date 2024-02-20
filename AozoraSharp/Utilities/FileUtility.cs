using System.IO;

namespace AozoraSharp.Utilities;

public static class FileUtility
{
    public static byte[] ReadBytesFromFileStream(FileStream stream, out int size)
    {
        byte[] bytes = new byte[stream.Length];
        size = stream.Read(bytes);
        return bytes;
    }
}
