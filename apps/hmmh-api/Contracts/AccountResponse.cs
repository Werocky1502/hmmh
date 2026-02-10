namespace Hmmh.Api.Contracts;

/// <summary>
///     Response payload for user management actions.
/// </summary>
public sealed class AccountResponse
{
    /// <summary>
    ///     Display login for the authenticated user.
    /// </summary>
    public string UserName { get; init; } = string.Empty;
}
