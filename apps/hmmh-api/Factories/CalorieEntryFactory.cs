using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="CalorieEntry" /> entities for persistence.
/// </summary>
public sealed class CalorieEntryFactory : ICalorieEntryFactory
{
    /// <inheritdoc />
    public CalorieEntry Create(
        Guid userId,
        DateOnly date,
        int calories,
        string? foodName,
        string? partOfDay,
        string? note)
    {
        // Create a new calorie entry with required identifiers.
        return new CalorieEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EntryDate = date,
            Calories = calories,
            FoodName = foodName,
            PartOfDay = partOfDay,
            Note = note,
        };
    }
}
