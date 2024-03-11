using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.AozoraObjects;
using AozoraSharp.Constants;
using AozoraSharp.Core;

namespace AozoraSharp.BlueskyModels;

public sealed class LazyUserPosts(AozoraUser user, bool reverse, int intervalMilliseconds) : LazyList<AozoraPost>
{
    private readonly AozoraUser user = user;
    private readonly bool reverse = reverse;
    private readonly int intervalMilliseconds = intervalMilliseconds;
    private DateTime lastFetchTimeUtc = DateTime.MinValue;
    private string cursor = default;
    private bool isEndOfFeed = false;

    protected override async Task<IReadOnlyList<AozoraPost>> GetMoreAsync(CancellationToken cancellationToken = default)
    {
        if (isEndOfFeed)
        {
            return null;
        }
        if (lastFetchTimeUtc != DateTime.MinValue)
        {
            var millisecondsSinceLastFetch = (int)(DateTime.UtcNow - lastFetchTimeUtc).TotalMilliseconds;
            var waitingMilliseconds = intervalMilliseconds - millisecondsSinceLastFetch;
            if (waitingMilliseconds > 0)
            {
                await Task.Delay(waitingMilliseconds, cancellationToken);
            }
        }
        var posts = await user.FetchPostsAsync(CommonConstant.ListRecordsMaxLimit, cursor, reverse, cancellationToken);
        lastFetchTimeUtc = DateTime.UtcNow;
        if (posts.Count <= 0)
        {
            isEndOfFeed = true;
            return null;
        }
        cursor = posts.Cursor;
        return posts;
    }
}
