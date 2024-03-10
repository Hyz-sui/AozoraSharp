using System.Collections.Generic;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

public record EmbedImages(IReadOnlyList<Image> Images) : Embed, IEmbedMedia;
