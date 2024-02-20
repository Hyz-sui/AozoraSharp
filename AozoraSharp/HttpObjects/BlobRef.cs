using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public readonly record struct BlobRef
{
    [JsonPropertyName("$link")]
    public string Link { get; init; }
}
