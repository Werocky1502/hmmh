using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Defines creation logic for <see cref="CalorieEntry" /> entities.
/// </summary>
public interface ICalorieEntryFactory
{
    /// <summary>
    ///     Creates a new calorie entry entity.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="date">Entry date.</param>
    /// <param name="calories">Calories recorded.</param>
    /// <param name="foodName">Optional food name.</param>
    /// <param name="partOfDay">Optional part of day.</param>
    /// <param name="note">Optional note.</param>
    /// <returns>New calorie entry entity.</returns>
    CalorieEntry Create(
        Guid userId,
        DateOnly date,
        int calories,
        string? foodName,
        string? partOfDay,
        string? note);
}
