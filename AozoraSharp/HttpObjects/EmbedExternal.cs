using AozoraSharp.Constants;
using AozoraSharp.Embeds;

namespace AozoraSharp.HttpObjects;

public record EmbedExternal(External External) : Embed
{
    public override string ATType { get; } = ATTypeName.EmbedExternal;
}
