using System.Security.Claims;
using Hmmh.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Hmmh.Api.Data;
using Hmmh.Api.Services;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles OAuth token exchanges for HMMH clients.
/// </summary>
[ApiController]
public sealed class ConnectController : ControllerBase
{
    private readonly ILogger<ConnectController> logger;
    private readonly HmmhDbContext dbContext;
    private readonly IPasswordHasherService passwordHasher;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConnectController" /> class.
    /// </summary>
    /// <param name="dbContext">Database context for user operations.</param>
    /// <param name="passwordHasher">Password hashing service.</param>
    /// <param name="logger">Logger for token events.</param>
    public ConnectController(
        HmmhDbContext dbContext,
        IPasswordHasherService passwordHasher,
        ILogger<ConnectController> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    /// <summary>
    ///     Exchanges credentials or refresh tokens for access tokens.
    /// </summary>
    /// <returns>OAuth token response handled by OpenIddict.</returns>
    [HttpPost("~/connect/token")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.Features.Get<OpenIddictServerAspNetCoreFeature>()?.Transaction?.Request
            ?? throw new InvalidOperationException("OpenIddict request cannot be resolved.");

        if (request.IsPasswordGrantType())
        {
            return await HandlePasswordGrantAsync(request);
        }

        if (request.IsRefreshTokenGrantType())
        {
            return await HandleRefreshGrantAsync(request);
        }

        return Forbid(BuildError(OpenIddictConstants.Errors.UnsupportedGrantType, "Unsupported grant type."),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandlePasswordGrantAsync(OpenIddictRequest request)
    {
        var login = NormalizeLogin(request.Username);
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.UserName == login);
        if (user is null)
        {
            return Forbid(BuildError(OpenIddictConstants.Errors.InvalidGrant, "Invalid login or password."),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var isValid = passwordHasher.VerifyPassword(request.Password ?? string.Empty, user.PasswordHash);
        if (!isValid)
        {
            return Forbid(BuildError(OpenIddictConstants.Errors.InvalidGrant, "Invalid login or password."),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var scopes = request.GetScopes().Intersect(new[] { "api", OpenIddictConstants.Scopes.OfflineAccess })
            .ToArray();
        var principal = BuildPrincipal(user, scopes);

        logger.LogInformation("Issued access token for user {UserId}.", user.Id);
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> HandleRefreshGrantAsync(OpenIddictRequest request)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
        {
            return Forbid(BuildError(OpenIddictConstants.Errors.InvalidGrant, "Invalid refresh token."),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var userIdValue = authenticateResult.Principal.FindFirstValue(OpenIddictConstants.Claims.Subject);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Forbid(BuildError(OpenIddictConstants.Errors.InvalidGrant, "User identifier is missing."),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(candidate => candidate.Id == userId);
        if (user is null)
        {
            return Forbid(BuildError(OpenIddictConstants.Errors.InvalidGrant, "User no longer exists."),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        var scopes = request.GetScopes();
        if (!scopes.Any())
        {
            scopes = authenticateResult.Principal.GetScopes();
        }

        var principal = BuildPrincipal(user, scopes);

        logger.LogInformation("Refreshed access token for user {UserId}.", user.Id);
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static ClaimsPrincipal BuildPrincipal(ApplicationUser user, IEnumerable<string> scopes)
    {
        var scopeList = scopes.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString("D"));
        identity.AddClaim(OpenIddictConstants.Claims.Name, user.UserName);

        foreach (var claim in identity.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, scopeList));
        }

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(scopeList);
        principal.SetResources("hmmh-api");
        return principal;
    }

    private static IEnumerable<string> GetDestinations(Claim claim, IReadOnlyCollection<string> scopes)
    {
        var destinations = new List<string> { OpenIddictConstants.Destinations.AccessToken };

        if (scopes.Contains(OpenIddictConstants.Scopes.OpenId)
            && (claim.Type == OpenIddictConstants.Claims.Subject || claim.Type == OpenIddictConstants.Claims.Name))
        {
            destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
        }

        return destinations;
    }

    private static AuthenticationProperties BuildError(string error, string description)
    {
        return new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictConstants.Parameters.Error] = error,
            [OpenIddictConstants.Parameters.ErrorDescription] = description,
        });
    }

    private static string NormalizeLogin(string? login)
    {
        return (login ?? string.Empty).Trim().ToLowerInvariant();
    }
}
