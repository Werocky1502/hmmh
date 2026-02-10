using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="WeightEntry" /> entities for persistence.
/// </summary>
public sealed class WeightEntryFactory : IWeightEntryFactory
{
    /// <inheritdoc />
    public WeightEntry Create(Guid userId, DateOnly date, decimal weightKg)
    {
        // Create a new weight entry with required identifiers.
        return new WeightEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EntryDate = date,
            WeightKg = weightKg,
        };
    }
}
