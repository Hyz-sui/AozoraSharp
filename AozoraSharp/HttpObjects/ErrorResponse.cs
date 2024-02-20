namespace AozoraSharp.HttpObjects;

/// <summary>
/// Represents a error response from atproto.
/// </summary>
public readonly record struct ErrorResponse(string Error, string Message);
