using AozoraSharp.HttpObjects;

namespace AozoraSharp.Embeds;

public readonly record struct External(string Uri, string Title, string Description, Blob? Thumb = null);
