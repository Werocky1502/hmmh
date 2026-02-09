namespace Hmmh.Api.Models;

/// <summary>
///     Stored weight entry for a user on a specific date.
/// </summary>
public sealed class WeightEntry
{
    /// <summary>
    ///     Unique identifier for the weight entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Foreign key to the owning user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    ///     Navigation to the owning user.
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    ///     Calendar date for the recorded weight.
    /// </summary>
    public DateOnly EntryDate { get; set; }

    /// <summary>
    ///     Weight in kilograms.
    /// </summary>
    public decimal WeightKg { get; set; }
}
