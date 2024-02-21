using AozoraSharp.Constants;
using AozoraSharp.HttpObjects.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public record EmbedImages(IReadOnlyList<EmbedImage> Images) : Embed
{
    public override string ATType { get; } = ATTypeName.EmbedImages;
}
