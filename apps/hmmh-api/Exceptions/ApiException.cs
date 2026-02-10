using Microsoft.AspNetCore.Http;

namespace Hmmh.Api.Exceptions;

/// <summary>
///     Base exception type for API errors with HTTP status codes.
/// </summary>
public abstract class ApiException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiException" /> class.
    /// </summary>
    /// <param name="message">Error message for the API client.</param>
    /// <param name="statusCode">HTTP status code associated with the error.</param>
    protected ApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    ///     Gets the HTTP status code that should be returned to the client.
    /// </summary>
    public int StatusCode { get; }
}

/// <summary>
///     Represents a 400 validation error in the API.
/// </summary>
public sealed class ValidationApiException : ApiException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationApiException" /> class.
    /// </summary>
    /// <param name="message">Validation error message.</param>
    public ValidationApiException(string message)
        : base(message, StatusCodes.Status400BadRequest)
    {
    }
}

/// <summary>
///     Represents a 404 not found error in the API.
/// </summary>
public sealed class NotFoundApiException : ApiException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundApiException" /> class.
    /// </summary>
    /// <param name="message">Not found error message.</param>
    public NotFoundApiException(string message)
        : base(message, StatusCodes.Status404NotFound)
    {
    }
}

/// <summary>
///     Represents a 409 conflict error in the API.
/// </summary>
public sealed class ConflictApiException : ApiException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConflictApiException" /> class.
    /// </summary>
    /// <param name="message">Conflict error message.</param>
    public ConflictApiException(string message)
        : base(message, StatusCodes.Status409Conflict)
    {
    }
}

/// <summary>
///     Represents a 401 unauthorized error in the API.
/// </summary>
public sealed class UnauthorizedApiException : ApiException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnauthorizedApiException" /> class.
    /// </summary>
    /// <param name="message">Unauthorized error message.</param>
    public UnauthorizedApiException(string message)
        : base(message, StatusCodes.Status401Unauthorized)
    {
    }
}
