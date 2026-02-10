using Hmmh.Api.Contracts.Responses;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="StatusResponse" /> payloads.
/// </summary>
public interface IStatusResponseFactory
{
    /// <summary>
    ///     Creates a status response payload.
    /// </summary>
    /// <param name="service">Service name.</param>
    /// <param name="status">Status description.</param>
    /// <param name="timestamp">Timestamp for the response.</param>
    /// <returns>Status response payload.</returns>
    StatusResponse Create(string service, string status, DateTimeOffset timestamp);
}
