using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Hmmh.Api.Services;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles OAuth token exchanges for HMMH clients.
/// </summary>
[ApiController]
public sealed class ConnectController : ControllerBase
{
    private readonly ITokenService tokenService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectController" /> class.
    /// </summary>
    public ConnectController(
        ITokenService tokenService)
    {
        this.tokenService = tokenService;
    }

    /// <summary>
    ///     Exchanges credentials or refresh tokens for access tokens.
    /// </summary>
    /// <returns>OAuth token response handled by OpenIddict.</returns>
    [AllowAnonymous]
    [HttpPost("~/connect/token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Exchange(CancellationToken cancellationToken)
    {
        // Dispatch the request to the appropriate token flow.
        var request = HttpContext.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction?.Request
            ?? throw new InvalidOperationException("OpenIddict request cannot be resolved.");

        if (request.IsPasswordGrantType())
        {
            var result = await tokenService.HandlePasswordGrantAsync(request, cancellationToken);
            return BuildTokenResponse(result);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await HandleRefreshGrantAsync(request, cancellationToken);
        }

        return Forbid(OpenIddictErrorBuilder.BuildError(OpenIddictConstants.Errors.UnsupportedGrantType, "Unsupported grant type."),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleRefreshGrantAsync(
        OpenIddictRequest request,
        CancellationToken cancellationToken)
    {
        // Validate the refresh token before issuing a new access token.
        var authenticateResult = await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
        {
            return Forbid(OpenIddictErrorBuilder.BuildError(OpenIddictConstants.Errors.InvalidGrant, "Invalid refresh token."),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var result = await tokenService.HandleRefreshGrantAsync(
            request,
            authenticateResult.Principal,
            cancellationToken);

        return BuildTokenResponse(result);
    }

    private static IActionResult BuildTokenResponse(TokenExchangeResult result)
    {
        // Convert the token exchange result into an HTTP response.
        if (result.IsSuccessful && result.Principal is not null)
        {
            return new SignInResult(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                result.Principal,
                null);
        }

        return new ForbidResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, result.ErrorProperties);
    }
}
