namespace Hmmh.Api.Services;

/// <summary>
///     Provides password hashing and verification for local users.
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    ///     Hashes a plain-text password.
    /// </summary>
    /// <param name="password">Plain-text password.</param>
    /// <returns>Encoded password hash payload.</returns>
    string HashPassword(string password);

    /// <summary>
    ///     Verifies a password against a stored hash payload.
    /// </summary>
    /// <param name="password">Plain-text password.</param>
    /// <param name="storedHash">Stored hash payload.</param>
    /// <returns>True when the password matches.</returns>
    bool VerifyPassword(string password, string storedHash);
}
