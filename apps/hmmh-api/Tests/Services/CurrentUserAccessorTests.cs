using System.Security.Claims;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Hmmh.Api.Tests.Services;

/// <summary>
///     Tests for <see cref="CurrentUserAccessor" />.
/// </summary>
[TestClass]
public sealed class CurrentUserAccessorTests
{
    /// <summary>
    ///     Ensures the user id claim is parsed correctly.
    /// </summary>
    [TestMethod]
    public void UserId_ReturnsParsedGuid()
    {
        // Verify the accessor reads the subject claim.
        var userId = Guid.NewGuid();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(OpenIddictConstants.Claims.Subject, userId.ToString("D")),
            }, "test")),
        };
        var accessor = new CurrentUserAccessor(new HttpContextAccessor { HttpContext = context });

        var result = accessor.UserId;

        Assert.AreEqual(userId, result);
    }

    /// <summary>
    ///     Ensures missing user id claims throw an exception.
    /// </summary>
    [TestMethod]
    public void UserId_ThrowsWhenMissing()
    {
        // Verify missing claims trigger an unauthorized exception.
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
        };
        var accessor = new CurrentUserAccessor(new HttpContextAccessor { HttpContext = context });

        Assert.ThrowsException<UnauthorizedAccessException>(() =>
        {
            _ = accessor.UserId;
        });
    }

    /// <summary>
    ///     Ensures authentication status is reflected by the accessor.
    /// </summary>
    [TestMethod]
    public void IsAuthenticated_ReturnsFalseForAnonymous()
    {
        // Verify anonymous identities return false for authentication status.
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()),
        };
        var accessor = new CurrentUserAccessor(new HttpContextAccessor { HttpContext = context });

        var isAuthenticated = accessor.IsAuthenticated;

        Assert.IsFalse(isAuthenticated);
    }
}
