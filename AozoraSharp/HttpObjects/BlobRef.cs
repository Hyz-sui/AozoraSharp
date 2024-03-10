using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public readonly record struct BlobRef([property: JsonPropertyName("$link")] string Link);
