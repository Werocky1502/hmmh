using System.ComponentModel.DataAnnotations;

namespace Hmmh.Api.Models;

/// <summary>
///     Authentication request payload for sign-in and sign-up.
/// </summary>
public sealed class AuthRequest
{
    /// <summary>
    ///     Login identifier for the user.
    /// </summary>
    [Required]
    [MinLength(3)]
    public string Login { get; init; } = string.Empty;

    /// <summary>
    ///     Plain-text password for authentication.
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;
}

/// <summary>
///     Authentication response containing the issued JWT token.
/// </summary>
public sealed class AuthResponse
{
    /// <summary>
    ///     JWT token for authenticated API access.
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    ///     Display login for the authenticated user.
    /// </summary>
    public string UserName { get; init; } = string.Empty;
}

/// <summary>
///     JWT configuration settings from app configuration.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    ///     Symmetric signing key for JWT tokens.
    /// </summary>
    [Required]
    public string Key { get; init; } = string.Empty;

    /// <summary>
    ///     Issuer claim for generated tokens.
    /// </summary>
    [Required]
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    ///     Audience claim for generated tokens.
    /// </summary>
    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    ///     Token lifetime in minutes.
    /// </summary>
    [Range(1, 1440)]
    public int TokenMinutes { get; init; } = 120;
}
