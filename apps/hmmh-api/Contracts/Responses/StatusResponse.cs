namespace Hmmh.Api.Contracts.Responses;

/// <summary>
///     Response payload for API status checks.
/// </summary>
public sealed record StatusResponse(string Service, string Status, DateTimeOffset Timestamp);
