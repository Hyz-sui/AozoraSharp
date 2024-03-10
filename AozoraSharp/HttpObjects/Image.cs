using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public readonly record struct Image(string Alt, [property: JsonPropertyName("image")] Blob ImageBlob);
