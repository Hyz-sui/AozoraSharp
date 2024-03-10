namespace AozoraSharp.Constants;

public static class ATEndpoint
{
    #region atproto

    // server / POST
    public const string CreateSession = "com.atproto.server.createSession";
    public const string RefreshSession = "com.atproto.server.refreshSession";
    public const string DeleteSession = "com.atproto.server.deleteSession";

    // repo / POST
    public const string CreateRecord = "com.atproto.repo.createRecord";
    public const string DeleteRecord = "com.atproto.repo.deleteRecord";
    public const string UploadBlob = "com.atproto.repo.uploadBlob";

    // repo / GET
    public const string ListRecords = "com.atproto.repo.listRecords";

    #endregion
    #region bsky

    // actor / GET
    public const string GetProfile = "app.bsky.actor.getProfile";

    #endregion
}
