using Hmmh.Api.Extensions;
using Hmmh.Api.Models;
using Hmmh.Api.Repositories;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace Hmmh.Api.Services;

/// <summary>
///     Handles OpenIddict token exchanges for the API.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly IRepository<ApplicationUser> userRepository;
    private readonly IPasswordHasherService passwordHasher;
    private readonly ILogger<TokenService> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TokenService" /> class.
    /// </summary>
    /// <param name="userRepository">Repository for user data.</param>
    /// <param name="passwordHasher">Password hashing service.</param>
    /// <param name="logger">Logger for token operations.</param>
    public TokenService(
        IRepository<ApplicationUser> userRepository,
        IPasswordHasherService passwordHasher,
        ILogger<TokenService> logger)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<TokenExchangeResult> HandlePasswordGrantAsync(OpenIddictRequest request, CancellationToken cancellationToken)
    {
        // Validate credentials and issue an access token when valid.
        var login = request.Username.NormalizeLogin();
        if (string.IsNullOrWhiteSpace(login))
        {
            return TokenExchangeResult.Fail(OpenIddictErrorBuilder.BuildError(
                OpenIddictConstants.Errors.InvalidGrant,
                "Invalid login or password."));
        }

        var user = await userRepository.FindAsync(userEntry => userEntry.UserName == login, false, cancellationToken);
        if (user is null)
        {
            return TokenExchangeResult.Fail(OpenIddictErrorBuilder.BuildError(
                OpenIddictConstants.Errors.InvalidGrant,
                "Invalid login or password."));
        }

        var isValid = passwordHasher.VerifyPassword(request.Password ?? string.Empty, user.PasswordHash);
        if (!isValid)
        {
            return TokenExchangeResult.Fail(OpenIddictErrorBuilder.BuildError(
                OpenIddictConstants.Errors.InvalidGrant,
                "Invalid login or password."));
        }

        var scopes = request.GetScopes().Intersect(new[]
        {
            OpenIddictConstants.Scopes.OpenId,
            "api",
            OpenIddictConstants.Scopes.OfflineAccess,
        })
            .ToArray();
        var principal = BuildPrincipal(user, scopes);

        logger.LogInformation("Issued access token for user {UserId}.", user.Id);
        return TokenExchangeResult.Success(principal);
    }

    /// <inheritdoc />
    public async Task<TokenExchangeResult> HandleRefreshGrantAsync(
        OpenIddictRequest request,
        ClaimsPrincipal refreshPrincipal,
        CancellationToken cancellationToken)
    {
        // Validate the refresh principal and issue a new token.
        var userIdValue = refreshPrincipal.FindFirstValue(OpenIddictConstants.Claims.Subject);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return TokenExchangeResult.Fail(OpenIddictErrorBuilder.BuildError(
                OpenIddictConstants.Errors.InvalidGrant,
                "User identifier is missing."));
        }

        var user = await userRepository.FindAsync(userEntry => userEntry.Id == userId, false, cancellationToken);
        if (user is null)
        {
            return TokenExchangeResult.Fail(OpenIddictErrorBuilder.BuildError(
                OpenIddictConstants.Errors.InvalidGrant,
                "User no longer exists."));
        }

        var scopes = request.GetScopes();
        if (!scopes.Any())
        {
            scopes = refreshPrincipal.GetScopes();
        }

        var principal = BuildPrincipal(user, scopes);

        logger.LogInformation("Refreshed access token for user {UserId}.", user.Id);
        return TokenExchangeResult.Success(principal);
    }

    private static ClaimsPrincipal BuildPrincipal(ApplicationUser user, IEnumerable<string> scopes)
    {
        // Build the claims principal for OpenIddict issuance.
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
        // Determine the destinations for each claim.
        var destinations = new List<string> { OpenIddictConstants.Destinations.AccessToken };

        if (scopes.Contains(OpenIddictConstants.Scopes.OpenId)
            && (claim.Type == OpenIddictConstants.Claims.Subject || claim.Type == OpenIddictConstants.Claims.Name))
        {
            destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
        }

        return destinations;
    }

}
