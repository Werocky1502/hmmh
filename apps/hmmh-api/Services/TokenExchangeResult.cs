using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Hmmh.Api.Services;

/// <summary>
///     Represents the outcome of a token exchange operation.
/// </summary>
public sealed record TokenExchangeResult
{
    private TokenExchangeResult(bool isSuccessful, ClaimsPrincipal? principal, AuthenticationProperties? errorProperties)
    {
        IsSuccessful = isSuccessful;
        Principal = principal;
        ErrorProperties = errorProperties;
    }

    /// <summary>
    ///     Gets whether the exchange was successful.
    /// </summary>
    public bool IsSuccessful { get; }

    /// <summary>
    ///     Gets the authenticated principal when the exchange succeeds.
    /// </summary>
    public ClaimsPrincipal? Principal { get; }

    /// <summary>
    ///     Gets the error properties when the exchange fails.
    /// </summary>
    public AuthenticationProperties? ErrorProperties { get; }

    /// <summary>
    ///     Creates a successful exchange result.
    /// </summary>
    /// <param name="principal">Authenticated principal.</param>
    /// <returns>A successful exchange result.</returns>
    public static TokenExchangeResult Success(ClaimsPrincipal principal)
    {
        // Return a success result with the authenticated principal.
        return new TokenExchangeResult(true, principal, null);
    }

    /// <summary>
    ///     Creates a failed exchange result.
    /// </summary>
    /// <param name="errorProperties">Error details for the failure.</param>
    /// <returns>A failed exchange result.</returns>
    public static TokenExchangeResult Fail(AuthenticationProperties errorProperties)
    {
        // Return a failure result with error metadata.
        return new TokenExchangeResult(false, null, errorProperties);
    }
}
