using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.BlueskyModels;
using AozoraSharp.Constants;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;

namespace AozoraSharp.AozoraObjects;

/// <summary>
/// Represents an account.
/// </summary>
public class AozoraUser(
    string handle,
    string did,
    string displayName,
    string description,
    string avatarUrl,
    string bannerUrl,
    int followersCount,
    int followsCount,
    int postsCount,
    ProfileAssociatedInfo associatedInfo,
    ProfileLabelsInfo labelsInfo) : AozoraObject
{
    /// <summary>
    /// Handle of the account.<br/>e.g. hyze.bsky.social
    /// </summary>
    public string Handle { get; } = handle;
    /// <summary>
    /// Did of the account.<br/>e.g. did:plc:foobar
    /// </summary>
    public string Did { get; } = did;
    public string DisplayName { get; } = displayName;
    public string Description { get; } = description;
    public string AvatarUrl { get; } = avatarUrl;
    public string BannerUrl { get; } = bannerUrl;
    public int FollowersCount { get; } = followersCount;
    public int FollowsCount { get; } = followsCount;
    public int PostsCount { get; } = postsCount;
    public ProfileAssociatedInfo AssociatedInfo { get; } = associatedInfo;
    public ProfileLabelsInfo LabelsInfo { get; } = labelsInfo;

    public async Task<FetchedPosts> FetchPostsAsync(AozoraClient worker, int limit = 50, string cursor = null, bool reverse = false, CancellationToken cancellationToken = default)
    {
        logger.Debug("fetching posts");
        var response = await worker.GetCustomXrpcAsync<FetchPostsResponse>(
            ATEndpoint.ListRecords,
            [
                new("repo", Did),
                new("collection", RecordTypeName.Post),
                new("limit", limit.ToString()),
                new("cursor", cursor),
                new("reverse", reverse.ToString()),
            ],
            cancellationToken);
        var records = response.Records;
        var posts = new AozoraPost[records.Count];
        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            if (record.Value is not Post post)
            {
                throw new InvalidRecordValueException(typeof(Post), record.Value?.GetType());
            }
            var aozoraPost = new AozoraPost(this, record.Uri, record.Cid, post);
            posts[i] = aozoraPost;
        }
        var fetchedPosts = new FetchedPosts(response.Cursor, posts);
        return fetchedPosts;
    }
    public LazyUserPosts GetLazyPosts(AozoraClient worker, bool reverse = false, int intervalMilliseconds = 5000)
    {
        return new LazyUserPosts(worker, this, reverse, intervalMilliseconds);
    }

    public async Task<Record> FetchRecordAsync(AozoraClient worker, string collection, string recordKey, CancellationToken cancellationToken = default)
    {
        var record = await worker.GetCustomXrpcAsync<Record>("com.atproto.repo.getRecord", [new("repo", Did), new("collection", collection), new("rkey", recordKey)], cancellationToken);
        return record;
    }
}
