using System;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.AozoraObjects;
using AozoraSharp.Constants;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Records;
using AozoraSharp.Utilities;

namespace AozoraSharp.BlueskyModels;

public readonly struct UserRelationship(
    AozoraMyUser me,
    AozoraUser subject,
    bool amMuting,
    UserList? muteList,
    bool amBlocked/*blockedBy*/,
    bool amBlocking,
    string blockRecordUri,
    UserList? blockList,
    bool amFollowing,
    string followRecordUri,
    bool amFollowedBy,
    string followedByRecordUri)
{
    /// <summary>
    /// the viewer
    /// </summary>
    public AozoraMyUser Me { get; } = me;
    /// <summary>
    /// the subject user
    /// </summary>
    public AozoraUser Subject { get; } = subject;
    /// <summary>
    /// whether the viewer is muting the subject
    /// </summary>
    public bool AmMuting { get; } = amMuting;
    public UserList? MuteList { get; } = muteList;
    /// <summary>
    /// whether the viewer is blocked by the subject
    /// </summary>
    public bool AmBlocked { get; } = amBlocked;
    /// <summary>
    /// whether the viewer is blocking the subject
    /// </summary>
    public bool AmBlocking { get; } = amBlocking;
    internal string BlockRecordUri { get; } = blockRecordUri;
    public UserList? BlockList { get; } = blockList;
    /// <summary>
    /// whether the viewer is following the subject
    /// </summary>
    public bool AmFollowing { get; } = amFollowing;
    internal string FollowRecordUri { get; } = followRecordUri;

    /// <summary>
    /// whether the viewer is followed by the subject
    /// </summary>
    public bool AmFollowedBy { get; } = amFollowedBy;
    internal string FollowedByRecordUri { get; } = followedByRecordUri;

    public UserRelationship(AozoraMyUser me, AozoraUser subject, ProfileViewer profileViewer) : this(
        me,
        subject,
        profileViewer.Muted,
        profileViewer.MutedByList,
        profileViewer.BlockedBy,
        profileViewer.Blocking != null,
        profileViewer.Blocking,
        profileViewer.BlockingByList,
        profileViewer.Following != null,
        profileViewer.Following,
        profileViewer.FollowedBy != null,
        profileViewer.FollowedBy)
    { }

    public async Task<DateTime?> FetchBlockStartedAtAsync(CancellationToken cancellationToken = default)
    {
        if (BlockRecordUri == null)
        {
            return null;
        }
        var record = await Me.FetchRecordAsync(ATTypeName.GraphBlock, ATUtility.UriToRecordKey(BlockRecordUri), cancellationToken);
        return record.Value is BlockRecord blockRecord
            ? blockRecord.CreatedAt
            : throw new InvalidRecordValueException(typeof(BlockRecord), record.Value?.GetType());
    }
    public async Task<DateTime?> FetchFollowingStartedAtAsync(CancellationToken cancellationToken = default)
    {
        if (FollowRecordUri == null)
        {
            return null;
        }
        var record = await Me.FetchRecordAsync(ATTypeName.GraphFollow, ATUtility.UriToRecordKey(FollowRecordUri), cancellationToken);
        return record.Value is FollowRecord followRecord
            ? followRecord.CreatedAt
            : throw new InvalidRecordValueException(typeof(FollowRecord), record.Value?.GetType());
    }
    public async Task<DateTime?> FetchFollowedStartedAtAsync(CancellationToken cancellationToken = default)
    {
        if (FollowedByRecordUri == null)
        {
            return null;
        }
        var record = await Subject.FetchRecordAsync(ATTypeName.GraphFollow, ATUtility.UriToRecordKey(FollowedByRecordUri), cancellationToken);
        return record.Value is FollowRecord followRecord
            ? followRecord.CreatedAt
            : throw new InvalidRecordValueException(typeof(FollowRecord), record.Value?.GetType());
    }
}
