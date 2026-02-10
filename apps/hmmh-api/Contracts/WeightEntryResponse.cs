namespace Hmmh.Api.Contracts;

/// <summary>
///     Response payload for a stored weight entry.
/// </summary>
public sealed class WeightEntryResponse
{
    /// <summary>
    ///     Unique identifier for the weight entry.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     Date for the weight entry.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    ///     Weight in kilograms.
    /// </summary>
    public decimal WeightKg { get; init; }
}
