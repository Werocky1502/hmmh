using OpenIddict.Abstractions;
using System.Security.Claims;

namespace Hmmh.Api.Services;

/// <summary>
///     Defines token exchange workflows for the API.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     Handles password grant exchanges.
    /// </summary>
    /// <param name="request">OpenIddict request payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Token exchange result.</returns>
    Task<TokenExchangeResult> HandlePasswordGrantAsync(OpenIddictRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles refresh token exchanges.
    /// </summary>
    /// <param name="request">OpenIddict request payload.</param>
    /// <param name="refreshPrincipal">Authenticated principal from refresh token.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Token exchange result.</returns>
    Task<TokenExchangeResult> HandleRefreshGrantAsync(
        OpenIddictRequest request,
        ClaimsPrincipal refreshPrincipal,
        CancellationToken cancellationToken);
}
