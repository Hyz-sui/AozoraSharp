namespace AozoraSharp.Constants;

public static class ATEndpoint
{
    public const string CreateSession = "com.atproto.server.createSession";
    public const string RefreshSession = "com.atproto.server.refreshSession";
    public const string DeleteSession = "com.atproto.server.deleteSession";

    public const string CreateRecord = "com.atproto.repo.createRecord";
    public const string DeleteRecord = "com.atproto.repo.deleteRecord";
    public const string UploadBlob = "com.atproto.repo.uploadBlob";
}
