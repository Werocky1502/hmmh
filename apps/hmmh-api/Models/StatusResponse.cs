namespace Hmmh.Api.Models;

/// <summary>
///     Response payload for API status checks.
/// </summary>
public sealed record StatusResponse(string Service, string Status, DateTimeOffset Timestamp);
