using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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

/// <summary>
///     PBKDF2-based password hashing implementation.
/// </summary>
public sealed class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    /// <inheritdoc />
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            Iterations,
            KeySize);

        return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        var parts = storedHash.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            iterations,
            expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
