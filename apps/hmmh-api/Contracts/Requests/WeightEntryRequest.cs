using System.ComponentModel.DataAnnotations;

namespace Hmmh.Api.Contracts.Requests;

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

