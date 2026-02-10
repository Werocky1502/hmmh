using Hmmh.Api.Services;

namespace Hmmh.Api.Tests.Services;

/// <summary>
///     Tests for <see cref="PasswordHasherService" />.
/// </summary>
[TestClass]
public sealed class PasswordHasherServiceTests
{
    /// <summary>
    ///     Ensures hashing returns a non-empty hash.
    /// </summary>
    [TestMethod]
    public void HashPassword_ReturnsHashValue()
    {
        // Verify hashing returns a non-empty string.
        var service = new PasswordHasherService();

        var hash = service.HashPassword("p@ssword!");

        Assert.IsFalse(string.IsNullOrWhiteSpace(hash));
    }

    /// <summary>
    ///     Ensures validation succeeds for the correct password.
    /// </summary>
    [TestMethod]
    public void VerifyPassword_ReturnsTrueForValidPassword()
    {
        // Verify the stored hash validates the original password.
        var service = new PasswordHasherService();
        var hash = service.HashPassword("p@ssword!");

        var isValid = service.VerifyPassword("p@ssword!", hash);

        Assert.IsTrue(isValid);
    }

    /// <summary>
    ///     Ensures validation fails for the wrong password.
    /// </summary>
    [TestMethod]
    public void VerifyPassword_ReturnsFalseForInvalidPassword()
    {
        // Verify mismatched passwords return false.
        var service = new PasswordHasherService();
        var hash = service.HashPassword("p@ssword!");

        var isValid = service.VerifyPassword("wrong", hash);

        Assert.IsFalse(isValid);
    }
}
