using AozoraSharp.Constants;
using AozoraSharp.Embeds;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;
using AozoraSharp.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AozoraSharp.AozoraObjects;

public class AozoraPost(AozoraUser author, string text, DateTime createdAt, string collection, string uri, string cid, IEmbed embed = null) : AozoraObject
{
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
    /// collection that the post belongs
    /// </summary>
    public string Collection { get; } = collection;
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
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, IEmbed embed = null, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        logger.Debug("Creating reply");
        var root = ReplyRoot ?? this;
        return await author.CreateReplyAsync(this, root, text, langs, embed, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with images.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">reply text</param>
    /// <param name="langs">language codes of the reply</param>
    /// <param name="images">images to attach</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, IReadOnlyList<ImageInfo> images, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var embedImages = await author.UploadImagesAsync(images, cancellationToken);
        return await ReplyAsync(author, text, langs, embedImages, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote reply.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, AozoraPost postToQuote, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var embedPost = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        return await ReplyAsync(author, text, langs, embedPost, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="uri">link to external website</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, string uri, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var externalInfo = await WebUtility.FetchExternalInfoFromUriAsync(uri, author.Client.HttpClient);
        return await ReplyAsync(author, text, langs, externalInfo, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with custom website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="externalInfo">informations of the website card</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, ExternalInfo externalInfo, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var thumb = await externalInfo.UploadThumbnailAsync(author, cancellationToken);
        var external = new External(externalInfo.Uri, externalInfo.Title, externalInfo.Description, thumb);
        var embedExternal = new EmbedExternal(external);
        return await ReplyAsync(author, text, langs, embedExternal, collection, cancellationToken);
    }
    /// <summary>
    /// Create a reply with website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="uri">link to external website</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, string uri, AozoraPost postToQuote, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var externalInfo = await WebUtility.FetchExternalInfoFromUriAsync(uri, author.Client.HttpClient);
        return await ReplyAsync(author, text, langs, externalInfo, postToQuote, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote reply with custom website card.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="externalInfo">informations of the website card</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, ExternalInfo externalInfo, AozoraPost postToQuote, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var embedRecord = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        var thumb = await externalInfo.UploadThumbnailAsync(author, cancellationToken);
        var external = new External(externalInfo.Uri, externalInfo.Title, externalInfo.Description, thumb);
        var embedExternal = new EmbedExternal(external);
        var embedRecordWithExternal = new EmbedRecordWithExternal(embedRecord, embedExternal);
        return await ReplyAsync(author, text, langs, embedRecordWithExternal, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote reply with images.
    /// </summary>
    /// <param name="author">author of your reply</param>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="images">informations of the images</param>
    /// <param name="collection">Collection that you want to add the post to.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> ReplyAsync(AozoraMyUser author, string text, ICollection<string> langs, IReadOnlyList<ImageInfo> images, AozoraPost postToQuote, string collection = CommonConstant.DefaultPostCollection, CancellationToken cancellationToken = default)
    {
        var embedRecord = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        var embedImages = await author.UploadImagesAsync(images, cancellationToken);
        var embedRecordWithImages = new EmbedRecordWithImages(embedRecord, embedImages);
        return await ReplyAsync(author, text, langs, embedRecordWithImages, collection, cancellationToken);
    }
}
