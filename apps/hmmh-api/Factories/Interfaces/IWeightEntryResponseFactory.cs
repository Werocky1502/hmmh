using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="WeightEntryResponse" /> payloads.
/// </summary>
public interface IWeightEntryResponseFactory
{
    /// <summary>
    ///     Creates a response from a weight entry entity.
    /// </summary>
    /// <param name="entry">Weight entry entity.</param>
    /// <returns>Weight response payload.</returns>
    WeightEntryResponse Create(WeightEntry entry);

    /// <summary>
    ///     Creates an empty response for a date without data.
    /// </summary>
    /// <param name="date">Entry date.</param>
    /// <returns>Empty weight response payload.</returns>
    WeightEntryResponse CreateEmpty(DateOnly date);
}
