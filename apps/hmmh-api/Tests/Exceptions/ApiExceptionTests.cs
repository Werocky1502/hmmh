using Hmmh.Api.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Hmmh.Api.Tests.Exceptions;

/// <summary>
///     Tests for API exception types.
/// </summary>
[TestClass]
public sealed class ApiExceptionTests
{
    /// <summary>
    ///     Ensures validation exceptions use the correct status code.
    /// </summary>
    [TestMethod]
    public void ValidationApiException_UsesBadRequestStatus()
    {
        // Verify validation status code is set to 400.
        var exception = new ValidationApiException("Invalid");

        Assert.AreEqual(StatusCodes.Status400BadRequest, exception.StatusCode);
        Assert.AreEqual("Invalid", exception.Message);
    }

    /// <summary>
    ///     Ensures not found exceptions use the correct status code.
    /// </summary>
    [TestMethod]
    public void NotFoundApiException_UsesNotFoundStatus()
    {
        // Verify not found status code is set to 404.
        var exception = new NotFoundApiException("Missing");

        Assert.AreEqual(StatusCodes.Status404NotFound, exception.StatusCode);
        Assert.AreEqual("Missing", exception.Message);
    }

    /// <summary>
    ///     Ensures conflict exceptions use the correct status code.
    /// </summary>
    [TestMethod]
    public void ConflictApiException_UsesConflictStatus()
    {
        // Verify conflict status code is set to 409.
        var exception = new ConflictApiException("Conflict");

        Assert.AreEqual(StatusCodes.Status409Conflict, exception.StatusCode);
        Assert.AreEqual("Conflict", exception.Message);
    }

    /// <summary>
    ///     Ensures unauthorized exceptions use the correct status code.
    /// </summary>
    [TestMethod]
    public void UnauthorizedApiException_UsesUnauthorizedStatus()
    {
        // Verify unauthorized status code is set to 401.
        var exception = new UnauthorizedApiException("Unauthorized");

        Assert.AreEqual(StatusCodes.Status401Unauthorized, exception.StatusCode);
        Assert.AreEqual("Unauthorized", exception.Message);
    }
}
