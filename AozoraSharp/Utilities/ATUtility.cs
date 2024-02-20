namespace AozoraSharp.Utilities;

public static class ATUtility
{
    public static string UriToRecordKey(string uri)
    {
        var parts = uri.Split("/");
        return parts[^1];
    }
}
