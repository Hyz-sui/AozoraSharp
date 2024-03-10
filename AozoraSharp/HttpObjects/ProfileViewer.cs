namespace AozoraSharp.HttpObjects;

public readonly record struct ProfileViewer(
    bool Muted,
    UserList MutedByList,
    bool BlockedBy,
    string Blocking,
    UserList BlockingByList,
    string Following,
    string FollowedBy);
