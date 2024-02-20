namespace AozoraSharp.HttpObjects;

public readonly record struct RefreshSessionResponse(string AccessJwt, string RefreshJwt, string Handle, string Did);
