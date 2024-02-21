using AozoraSharp.Constants;
using System.Collections.Generic;

namespace AozoraSharp.HttpObjects;

public record EmbedImages(IReadOnlyList<EmbedImage> Images) : Embed
{
    public override string ATType { get; } = ATTypeName.EmbedImages;
}
