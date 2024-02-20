using AozoraSharp.HttpObjects.Interfaces;
using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public abstract record Embed : IEmbed
{
    [JsonPropertyName("$type")]
    public abstract string ATType { get; }
}
