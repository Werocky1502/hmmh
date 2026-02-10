using System.Security.Claims;
using OpenIddict.Abstractions;

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

/// <summary>
///     Default implementation of <see cref="ICurrentUserAccessor" />.
/// </summary>
public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CurrentUserAccessor" /> class.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(OpenIddictConstants.Claims.Subject);
            if (!Guid.TryParse(userId, out var parsed))
            {
                throw new UnauthorizedAccessException("User identifier is missing from the access token.");
            }

            return parsed;
        }
    }

    /// <inheritdoc />
    public string? UserName => httpContextAccessor.HttpContext?.User?.FindFirstValue(OpenIddictConstants.Claims.Name);

    /// <inheritdoc />
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
