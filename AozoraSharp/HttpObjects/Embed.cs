using System.Text.Json.Serialization;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

// may be removed in the future
#pragma warning disable S2094
[JsonConverter(typeof(IEmbed))]
public abstract record Embed : IEmbed;
#pragma warning restore S2094
