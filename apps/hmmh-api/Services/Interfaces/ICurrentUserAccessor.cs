namespace Hmmh.Api.Services;

/// <summary>
///     Accesses current user claims for authenticated requests.
/// </summary>
public interface ICurrentUserAccessor
{
    /// <summary>
    ///     Gets the user identifier from the access token.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    ///     Gets the user display name from the access token.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    ///     Gets whether the request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
