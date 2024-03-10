using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.BlueskyModels;
using AozoraSharp.Constants;
using AozoraSharp.Embeds;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;
using AozoraSharp.Utilities;
using MimeMapping;

namespace AozoraSharp.AozoraObjects;

public class AozoraMyUser(CreateSessionResponse createSessionResponse, AozoraClient client, Profile myProfile) : AozoraUser(
    createSessionResponse.Handle,
    createSessionResponse.Did,
    myProfile.DisplayName,
    myProfile.Description,
    myProfile.Avatar,
    myProfile.Banner,
    myProfile.FollowersCount,
    myProfile.FollowsCount,
    myProfile.PostsCount,
    new(myProfile.Associated),
    new(myProfile.Labels))
{
    /// <summary>
    /// client that user belongs
    /// </summary>
    public AozoraClient Client { get; } = client;
    /// <summary>
    /// email address of the account
    /// </summary>
    public string Email { get; } = createSessionResponse.Email;
    /// <summary>
    /// whether the email address is confirmed
    /// </summary>
    public bool EmailConfirmed { get; } = createSessionResponse.EmailConfirmed;

    /// <summary>
    /// Create a post.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, IEmbed embed = null, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        logger.Debug("Creating post");
        var httpPost = new Post(text, DateTime.UtcNow.ToString("o"), langs, embed, Via: Client.Option.PostVia);
        var request = new CreatePostRequest(Did, collection, httpPost);
        var response = await Client.PostCustomXrpcAsync<CreatePostRequest, CreateRecordResponse>(ATEndpoint.CreateRecord, request, cancellationToken);
        var aozoraPost = new AozoraMyPost(this, httpPost, response);
        return aozoraPost;
    }
    /// <summary>
    /// Create a post with images.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="images">informations of the images</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, IReadOnlyList<ImageInfo> images, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embed = await UploadImagesAsync(images, cancellationToken);
        return await CreatePostAsync(text, langs, embed, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote post.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, AozoraPost postToQuote, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embed = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        return await CreatePostAsync(text, langs, embed, collection, cancellationToken);
    }
    /// <summary>
    /// Create a post with website card.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="uri">link to external website</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, string uri, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var externalInfo = await WebUtility.FetchExternalInfoFromUriAsync(uri, Client.HttpClient);
        return await CreatePostAsync(text, langs, externalInfo, collection, cancellationToken);
    }
    /// <summary>
    /// Create a post with custom website card.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="externalInfo">informations of the website card</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, ExternalInfo externalInfo, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var thumb = await externalInfo.UploadThumbnailAsync(this, cancellationToken);
        var external = new External(externalInfo.Uri, externalInfo.Title, externalInfo.Description, thumb);
        var embed = new EmbedExternal(external);
        return await CreatePostAsync(text, langs, embed, collection, cancellationToken);
    }
    /// <summary>
    /// Create a post with website card.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="uri">link to external website</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, string uri, AozoraPost postToQuote, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var externalInfo = await WebUtility.FetchExternalInfoFromUriAsync(uri, Client.HttpClient);
        return await CreatePostAsync(text, langs, externalInfo, postToQuote, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote post with custom website card.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="externalInfo">informations of the website card</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, ExternalInfo externalInfo, AozoraPost postToQuote, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embedRecord = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        var thumb = await externalInfo.UploadThumbnailAsync(this, cancellationToken);
        var external = new External(externalInfo.Uri, externalInfo.Title, externalInfo.Description, thumb);
        var embedExternal = new EmbedExternal(external);
        var embed = new EmbedRecordWithMedia(embedRecord, embedExternal);
        return await CreatePostAsync(text, langs, embed, collection, cancellationToken);
    }
    /// <summary>
    /// Create a quote post with images.
    /// </summary>
    /// <param name="text">post text</param>
    /// <param name="langs">language codes of the post</param>
    /// <param name="postToQuote">post to quote</param>
    /// <param name="images">informations of the images</param>
    /// <param name="collection">Record type.<br/>Normally, you don't have to change this.</param>
    /// <returns>created post</returns>
    public async Task<AozoraMyPost> CreatePostAsync(string text, ICollection<string> langs, IReadOnlyList<ImageInfo> images, AozoraPost postToQuote, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var embedRecord = new EmbedRecord(new(postToQuote.Uri, postToQuote.Cid));
        var embedImages = await UploadImagesAsync(images, cancellationToken);
        var embed = new EmbedRecordWithMedia(embedRecord, embedImages);
        return await CreatePostAsync(text, langs, embed, collection, cancellationToken);
    }

    // TODO: should be moved
    public async Task<AozoraUser> FetchUserAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var profile = await Client.GetCustomXrpcAsync<Profile>(ATEndpoint.GetProfile, [new AozoraClient.UrlParameter("actor", identifier)], cancellationToken);
        var user = new AozoraUser(
            profile.Handle,
            profile.Did,
            profile.DisplayName,
            profile.Description,
            profile.Avatar,
            profile.Banner,
            profile.FollowersCount,
            profile.FollowsCount,
            profile.PostsCount,
            new(profile.Associated),
            new(profile.Labels));
        return user;
    }
    public async Task<UserRelationship> FetchRelationshipAsync(AozoraUser user, CancellationToken cancellationToken = default)
    {
        var profile = await Client.GetCustomXrpcAsync<Profile>(ATEndpoint.GetProfile, [new AozoraClient.UrlParameter("actor", user.Did)], cancellationToken);
        var relationship = new UserRelationship(this, user, profile.Viewer);
        return relationship;
    }

    /// <summary>
    /// Upload images.
    /// </summary>
    /// <param name="images">images to upload</param>
    /// <returns>uploaded images</returns>
    public async Task<EmbedImages> UploadImagesAsync(IReadOnlyList<ImageInfo> images, CancellationToken cancellationToken = default)
    {
        logger.Debug($"Uploading {images.Count} images");
        var embedImages = new Image[images.Count];
        for (var i = 0; i < images.Count; i++)
        {
            var image = images[i];
            embedImages[i] = await UploadImageAsync(image, cancellationToken);
        }
        logger.Debug("Finished uploading images");
        return new(embedImages);
    }
    /// <summary>
    /// Upload an image.
    /// </summary>
    /// <param name="image">image to upload</param>
    /// <returns>uploaded image</returns>
    public async Task<Image> UploadImageAsync(ImageInfo image, CancellationToken cancellationToken = default)
    {
        var blob = await UploadImageBlobAsync(image.Path, cancellationToken);
        return new(image.Alt, blob);
    }
    internal async Task<Blob> UploadImageBlobAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        logger.Debug("Uploading image");
        using var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var bytes = FileUtility.ReadBytesFromFileStream(stream, out var size);
        logger.Debug($"{size} bytes");
        var mimeType = MimeUtility.GetMimeMapping(imagePath);
        var blob = await UploadBlobAsync(bytes, mimeType, cancellationToken: cancellationToken);
        logger.Debug("Finished uploading image");
        return blob;
    }
    internal async Task<Blob> UploadBlobAsync(IReadOnlyList<byte> bytes, string mimeType, CancellationToken cancellationToken = default)
    {
        logger.Debug("Uploading blob");
        var response = await Client.PostCustomXrpcAsync<UploadBlobResponse>(ATEndpoint.UploadBlob, mimeType, bytes, cancellationToken);
        logger.Debug("Finished uploading blob");
        return response.Blob;
    }
    internal async Task<AozoraMyPost> CreateReplyAsync(AozoraPost parent, AozoraPost root, string text, ICollection<string> langs, IEmbed embed = null, string collection = RecordTypeName.Post, CancellationToken cancellationToken = default)
    {
        var parentRef = new RecordStrongReference(parent.Uri, parent.Cid);
        var rootRef = new RecordStrongReference(root.Uri, root.Cid);
        var reply = new Reply(rootRef, parentRef);
        var httpPost = new Post(text, DateTime.UtcNow.ToString("o"), langs, embed, reply, Client.Option.PostVia);
        var request = new CreatePostRequest(Did, collection, httpPost);
        var response = await Client.PostCustomXrpcAsync<CreatePostRequest, CreateRecordResponse>(ATEndpoint.CreateRecord, request, cancellationToken);
        var aozoraPost = new AozoraMyPost(this, httpPost, response)
        {
            ReplyParent = parent,
            ReplyRoot = root,
        };
        return aozoraPost;
    }
}
