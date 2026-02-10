using Hmmh.Api.Exceptions;
using System.Text.Json;

namespace Hmmh.Api.Middleware;

/// <summary>
///     Handles API exceptions and converts them into consistent error responses.
/// </summary>
public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ApiExceptionMiddleware> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiExceptionMiddleware" /> class.
    /// </summary>
    /// <param name="next">Next middleware in the pipeline.</param>
    /// <param name="logger">Logger for exception events.</param>
    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    /// <summary>
    ///     Executes the middleware for each HTTP request.
    /// </summary>
    /// <param name="context">HTTP context for the request.</param>
    /// <returns>Task representing the middleware execution.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException ex)
        {
            // Return a controlled error payload for known API exceptions.
            logger.LogWarning(ex, "API exception handled with status {StatusCode}.", ex.StatusCode);
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            // Return a generic error payload for unexpected exceptions.
            logger.LogError(ex, "Unhandled exception processing request.");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        // Serialize the error response to JSON.
        var payload = JsonSerializer.Serialize(new { message });

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(payload);
    }
}
