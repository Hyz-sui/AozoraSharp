using AozoraSharp.AozoraObjects;
using AozoraSharp.HttpObjects;
using System.Threading;
using System.Threading.Tasks;

namespace AozoraSharp.Embeds;

/// <summary>
/// Represents information of embed external web site card.
/// </summary>
/// <param name="uri">link to external web site</param>
/// <param name="title">title of embed</param>
/// <param name="description">description of embed</param>
/// <param name="thumbPath">Path to thumbnail of embed. Can be omitted.</param>
public readonly struct ExternalInfo(string uri, string title, string description, string thumbPath = null)
{
    /// <summary>
    /// link to external web site
    /// </summary>
    public string Uri { get; } = uri;
    /// <summary>
    /// title of embed
    /// </summary>
    public string Title { get; } = title;
    /// <summary>
    /// description of embed
    /// </summary>
    public string Description { get; } = description;
    /// <summary>
    /// path to thumbnail of embed
    /// </summary>
    public string ThumbPath { get; } = thumbPath;

    internal async Task<Blob?> UploadThumbnailAsync(AozoraMyUser loginUser, CancellationToken cancellationToken = default)
    {
        return ThumbPath == null ? null : await loginUser.UploadImageBlobAsync(ThumbPath, cancellationToken);
    }
}
