namespace AozoraSharp.AozoraObjects;

/// <summary>
/// Represents an account.
/// </summary>
public class AozoraUser(string handle, string did, string email, bool emailConfirmed) : AozoraObject
{
    /// <summary>
    /// Handle of the account.<br/>e.g. hyze.bsky.social
    /// </summary>
    public string Handle { get; } = handle;
    /// <summary>
    /// Did of the account.<br/>e.g. did:plc:foobar
    /// </summary>
    public string Did { get; } = did;
    /// <summary>
    /// email address of the account
    /// </summary>
    public string Email { get; } = email;
    /// <summary>
    /// whether the email address is confirmed
    /// </summary>
    public bool EmailConfirmed { get; } = emailConfirmed;
}
