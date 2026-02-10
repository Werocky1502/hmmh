using Hmmh.Api.Contracts;
using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="CalorieEntryResponse" /> payloads.
/// </summary>
public interface ICalorieEntryResponseFactory
{
    /// <summary>
    ///     Creates a response from a calorie entry entity.
    /// </summary>
    /// <param name="entry">Calorie entry entity.</param>
    /// <returns>Calorie response payload.</returns>
    CalorieEntryResponse Create(CalorieEntry entry);
}
