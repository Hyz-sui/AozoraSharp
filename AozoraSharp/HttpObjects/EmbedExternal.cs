using AozoraSharp.Embeds;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

public record EmbedExternal(External External) : Embed, IEmbedMedia;
