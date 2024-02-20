namespace AozoraSharp.HttpObjects;

public readonly record struct CreateSessionResponse(string AccessJwt, string RefreshJwt, string Handle, string Did, string Email, bool EmailConfirmed);
