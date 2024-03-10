using System;

namespace AozoraSharp.HttpObjects;

public readonly record struct Profile(
    string Did,
    string Handle,
    string DisplayName,
    string Description,
    string Avatar,
    string Banner,
    int FollowersCount,
    int FollowsCount,
    int PostsCount,
    ProfileAssociated Associated,
    DateTime IndexedAt,
    ProfileViewer Viewer,
    ProfileLabel[] Labels);
