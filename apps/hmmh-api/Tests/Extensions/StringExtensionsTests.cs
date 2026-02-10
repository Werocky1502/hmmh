using Hmmh.Api.Extensions;

namespace Hmmh.Api.Tests.Extensions;

/// <summary>
///     Tests for <see cref="StringExtensions" />.
/// </summary>
[TestClass]
public sealed class StringExtensionsTests
{
    /// <summary>
    ///     Ensures login normalization trims and lowercases input.
    /// </summary>
    [TestMethod]
    public void NormalizeLogin_TrimsAndLowercases()
    {
        // Verify login normalization outputs trimmed lowercase text.
        var result = "  TestUser ".NormalizeLogin();

        Assert.AreEqual("testuser", result);
    }

    /// <summary>
    ///     Ensures optional text returns null for whitespace.
    /// </summary>
    [TestMethod]
    public void NormalizeOptionalText_ReturnsNullForWhitespace()
    {
        // Verify whitespace is normalized to null.
        var result = "  ".NormalizeOptionalText();

        Assert.IsNull(result);
    }
}
