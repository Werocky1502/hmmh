namespace Hmmh.Api.Db.Models;

/// <summary>
///     Stored calorie entry for a user on a specific date.
/// </summary>
public sealed class CalorieEntry
{
    /// <summary>
    ///     Unique identifier for the calorie entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Foreign key to the owning user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     Navigation to the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    ///     Calendar date for the recorded calories.
    /// </summary>
    public DateOnly EntryDate { get; set; }

    /// <summary>
    ///     Calories recorded for the entry.
    /// </summary>
    public int Calories { get; set; }

    /// <summary>
    ///     Optional name of the food item.
    /// </summary>
    public string? FoodName { get; set; }

    /// <summary>
    ///     Optional time of day label.
    /// </summary>
    public string? PartOfDay { get; set; }

    /// <summary>
    ///     Optional free-text note.
    /// </summary>
    public string? Note { get; set; }
}
