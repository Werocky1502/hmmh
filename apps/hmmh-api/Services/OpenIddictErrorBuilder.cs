using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;

namespace Hmmh.Api.Services;

/// <summary>
///     Builds OpenIddict error payloads for authentication failures.
/// </summary>
public static class OpenIddictErrorBuilder
{
    /// <summary>
    ///     Builds authentication properties for an OpenIddict error response.
    /// </summary>
    /// <param name="error">OpenIddict error code.</param>
    /// <param name="description">Human-readable error description.</param>
    /// <returns>Authentication properties for the error response.</returns>
    public static AuthenticationProperties BuildError(string error, string description)
    {
        // Build the error payload expected by OpenIddict.
        return new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictConstants.Parameters.Error] = error,
            [OpenIddictConstants.Parameters.ErrorDescription] = description,
        });
    }
}
