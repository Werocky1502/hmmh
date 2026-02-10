using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="WeightEntry" /> entities.
/// </summary>
public interface IWeightEntryFactory
{
    /// <summary>
    ///     Creates a new weight entry entity.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="date">Entry date.</param>
    /// <param name="weightKg">Weight in kilograms.</param>
    /// <returns>New weight entry entity.</returns>
    WeightEntry Create(Guid userId, DateOnly date, decimal weightKg);
}
