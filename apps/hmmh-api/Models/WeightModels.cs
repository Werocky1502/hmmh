using System.ComponentModel.DataAnnotations;

namespace Hmmh.Api.Models;

/// <summary>
///     Request payload for creating or updating a weight entry.
/// </summary>
public sealed class WeightEntryRequest
{
    /// <summary>
    ///     Date for the weight entry.
    /// </summary>
    [Required]
    public DateOnly Date { get; init; }

    /// <summary>
    ///     Weight in kilograms.
    /// </summary>
    [Range(20, 500)]
    public decimal WeightKg { get; init; }
}

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
