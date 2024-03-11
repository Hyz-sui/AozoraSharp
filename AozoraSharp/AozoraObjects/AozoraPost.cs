using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.Constants;
using AozoraSharp.Embeds;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;
using AozoraSharp.Utilities;

namespace AozoraSharp.AozoraObjects;

public class AozoraPost(
    AozoraMyUser myUser,
    AozoraUser author,
    string text,
    DateTime createdAt,
    string uri,
    string cid,
    IEmbed embed = default,
    string via = default) : AozoraObject
{
    public AozoraPost(AozoraMyUser myUser, AozoraUser author, string uri, string cid, Post post) : this(
        myUser,
        author,
        post.Text,
        post.CreatedAt,
        uri,
        cid,
        post.Embed,
        post.Via)
    { }

    public override AozoraMyUser MyUser { get; } = myUser;

    /// <summary>
    /// the author of the post
    /// </summary>
    public virtual AozoraUser Author { get; } = author;
    /// <summary>
    /// post text
    /// </summary>
    public string Text { get; } = text;
    /// <summary>
    /// time the post was created at
    /// </summary>
    public DateTime CreatedAt { get; } = createdAt;
    /// <summary>
    /// post uri
    /// </summary>
    public string Uri { get; } = uri;
    /// <summary>
    /// post cid
    /// </summary>
    public string Cid { get; } = cid;
    /// <summary>
    /// record key
    /// </summary>
    public string Rkey { get; } = ATUtility.UriToRecordKey(uri);
    /// <summary>
    /// post embed
    /// </summary>
    public IEmbed Embed { get; } = embed;
    public string Via { get; } = via;

    /// <summary>
    /// Reply parent of the post.<br/>null if the post is not a reply.
    /// </summary>
    public AozoraPost ReplyParent { get; init; } = null;
    /// <summary>
    /// Root of the reply tree that the post belongs.<br/>null if the post is not a reply.
    /// </summary>
    public AozoraPost ReplyRoot { get; init; } = null;

    /// <summary>
    /// Create a reply.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">reply text</param>
    /// <param name="langs">language codes of the reply</param>
    /// <param name="embed">embed to attach</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, IEmbed embed = null, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        logger.Debug("Creating reply");
        var root = ReplyRoot ?? this;
        return await author.CreateReplyAsync(this, root, text, langs, embed, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with images.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">reply text</param>
    /// <param name="langs">language codes of the reply</param>
    /// <param name="images">images to attach</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, IReadOnlyList<ImageInfo> images, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embedImages = await author.UploadImagesAsync(images, cancellationToken);
        return await ReplyAsync(author, text, langs, embedImages, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote reply.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, AozoraPost postToQuote, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embedPost = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        return await ReplyAsync(author, text, langs, embedPost, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="uri">link to external website</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, string uri, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var externalInfo = await WebUtility.FetchExternalInfoFromUriAsync(uri, author.Client.HttpClient);
        return await ReplyAsync(author, text, langs, externalInfo, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with custom website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="externalInfo">informations of the website card</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, ExternalInfo externalInfo, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var thumb = await externalInfo.UploadThumbnailAsync(author, cancellationToken);
        var external = new External(externalInfo.Uri, externalInfo.Title, externalInfo.Description, thumb);
        var embedExternal = new EmbedExternal(external);
        return await ReplyAsync(author, text, langs, embedExternal, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="uri">link to external website</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, string uri, AozoraPost postToQuote, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var externalInfo = await WebUtility.FetchExternalInfoFromUriAsync(uri, author.Client.HttpClient);
        return await ReplyAsync(author, text, langs, externalInfo, postToQuote, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote reply with custom website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="externalInfo">informations of the website card</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, ExternalInfo externalInfo, AozoraPost postToQuote, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embedRecord = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        var thumb = await externalInfo.UploadThumbnailAsync(author, cancellationToken);
        var external = new External(externalInfo.Uri, externalInfo.Title, externalInfo.Description, thumb);
        var embedExternal = new EmbedExternal(external);
        var embedRecordWithExternal = new EmbedRecordWithMedia(embedRecord, embedExternal);
        return await ReplyAsync(author, text, langs, embedRecordWithExternal, createdAt, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote reply with images.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="images">informations of the images</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, IReadOnlyList<ImageInfo> images, AozoraPost postToQuote, DateTime createdAt = default, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embedRecord = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        var embedImages = await author.UploadImagesAsync(images, cancellationToken);
        var embedRecordWithImages = new EmbedRecordWithMedia(embedRecord, embedImages);
        return await ReplyAsync(author, text, langs, embedRecordWithImages, createdAt, collection, cancellationToken);
    }
}
