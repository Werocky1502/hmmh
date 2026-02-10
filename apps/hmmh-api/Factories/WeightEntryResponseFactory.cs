using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="WeightEntryResponse" /> payloads for the API.
/// </summary>
public sealed class WeightEntryResponseFactory : IWeightEntryResponseFactory
{
    /// <inheritdoc />
    public WeightEntryResponse Create(WeightEntry entry)
    {
        // Map the entity to the API response contract.
        return new WeightEntryResponse
        {
            Id = entry.Id,
            Date = entry.EntryDate,
            WeightKg = entry.WeightKg,
        };
    }

    /// <inheritdoc />
    public WeightEntryResponse CreateEmpty(DateOnly date)
    {
        // Return an empty response for the requested date.
        return new WeightEntryResponse
        {
            Id = Guid.Empty,
            Date = date,
            WeightKg = 0,
        };
    }
}
