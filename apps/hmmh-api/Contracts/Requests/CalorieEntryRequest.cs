using System.ComponentModel.DataAnnotations;

namespace Hmmh.Api.Contracts.Requests;

/// <summary>
///     Request payload for creating a calorie entry.
/// </summary>
public sealed class CalorieEntryRequest
{
    /// <summary>
    ///     Date for the calorie entry.
    /// </summary>
    [Required]
    public DateOnly Date { get; init; }

    /// <summary>
    ///     Calories recorded for the entry.
    /// </summary>
    [Required]
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

