namespace AozoraSharp.Embeds;

/// <summary>
/// Represents information of an embed image.
/// </summary>
/// <param name="path">path to the image</param>
/// <param name="alt">alt text</param>
public readonly struct ImageInfo(string path, string alt)
{
    /// <summary>
    /// path to the image
    /// </summary>
    public string Path { get; } = path;
    /// <summary>
    /// alt text
    /// </summary>
    public string Alt { get; } = alt;
}
