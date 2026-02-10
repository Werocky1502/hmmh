using Hmmh.Api.Contracts;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="StatusResponse" /> payloads for the API.
/// </summary>
public sealed class StatusResponseFactory : IStatusResponseFactory
{
    /// <inheritdoc />
    public StatusResponse Create(string service, string status, DateTimeOffset timestamp)
    {
        // Build the status response payload.
        return new StatusResponse(service, status, timestamp);
    }
}
