using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Hmmh.Api.Services;

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
