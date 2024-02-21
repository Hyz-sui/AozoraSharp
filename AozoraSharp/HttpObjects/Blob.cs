using AozoraSharp.Constants;
using AozoraSharp.HttpObjects.Interfaces;
using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public readonly record struct Blob : IATType
{
    public BlobRef Ref { get; init; }
    public string MimeType { get; init; }
    public int Size { get; init; }

    [JsonConstructor]
    public Blob(BlobRef @ref, string mimeType, int size)
    {
        Ref = @ref;
        MimeType = mimeType;
        Size = size;
        ATType = ATTypeName.Blob;
    }

    [JsonPropertyName("$type")]
    public string ATType { get; } = ATTypeName.Blob;
}
