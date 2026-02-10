namespace Hmmh.Api.Contracts.Responses;

/// <summary>
///     Response payload for a stored calorie entry.
/// </summary>
public sealed class CalorieEntryResponse
{
    /// <summary>
    ///     Unique identifier for the calorie entry.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Date for the calorie entry.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    ///     Calories recorded for the entry.
    /// </summary>
    public int Calories { get; init; }

    /// <summary>
    ///     Optional name of the food item.
    /// </summary>
    public string? FoodName { get; init; }

    /// <summary>
    ///     Optional time of day label.
    /// </summary>
    public string? PartOfDay { get; init; }

    /// <summary>
    ///     Optional free-text note.
    /// </summary>
    public string? Note { get; init; }
}
