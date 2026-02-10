using System.Text.Json;
using Hmmh.Api.Exceptions;
using Hmmh.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hmmh.Api.Tests.Middleware;

/// <summary>
///     Tests for <see cref="ApiExceptionMiddleware" />.
/// </summary>
[TestClass]
public sealed class ApiExceptionMiddlewareTests
{
    /// <summary>
    ///     Ensures API exceptions are returned as JSON errors.
    /// </summary>
    [TestMethod]
    public async Task InvokeAsync_ReturnsApiExceptionResponse()
    {
        // Verify middleware handles API exceptions with the expected payload.
        var middleware = new ApiExceptionMiddleware(_ => throw new TestApiException("Bad input", 400), new NullLogger<ApiExceptionMiddleware>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = await JsonDocument.ParseAsync(context.Response.Body);
        var message = payload.RootElement.GetProperty("message").GetString();

        Assert.AreEqual(400, context.Response.StatusCode);
        Assert.AreEqual("Bad input", message);
    }

    /// <summary>
    ///     Ensures unexpected exceptions return a generic message.
    /// </summary>
    [TestMethod]
    public async Task InvokeAsync_ReturnsGenericErrorForUnhandledExceptions()
    {
        // Verify middleware handles generic exceptions with a 500 status.
        var middleware = new ApiExceptionMiddleware(_ => throw new InvalidOperationException("Boom"), new NullLogger<ApiExceptionMiddleware>());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var payload = await JsonDocument.ParseAsync(context.Response.Body);
        var message = payload.RootElement.GetProperty("message").GetString();

        Assert.AreEqual(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.AreEqual("An unexpected error occurred.", message);
    }

    private sealed class TestApiException : ApiException
    {
        public TestApiException(string message, int statusCode)
            : base(message, statusCode)
        {
            // Initialize test exception with custom status.
        }
    }
}
