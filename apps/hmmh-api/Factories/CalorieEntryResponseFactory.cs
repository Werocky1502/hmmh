using Hmmh.Api.Contracts;
using Hmmh.Api.Models;

namespace Hmmh.Api.Factories;

/// <summary>
///     Builds <see cref="CalorieEntryResponse" /> payloads for the API.
/// </summary>
public sealed class CalorieEntryResponseFactory : ICalorieEntryResponseFactory
{
    /// <inheritdoc />
    public CalorieEntryResponse Create(CalorieEntry entry)
    {
        // Map the entity to the API response contract.
        return new CalorieEntryResponse
        {
            Id = entry.Id,
            Date = entry.EntryDate,
            Calories = entry.Calories,
            FoodName = entry.FoodName,
            PartOfDay = entry.PartOfDay,
            Note = entry.Note,
        };
    }
}
