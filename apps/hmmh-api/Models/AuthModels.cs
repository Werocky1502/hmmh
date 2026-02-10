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

