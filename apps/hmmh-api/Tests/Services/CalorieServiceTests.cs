using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Exceptions;
using Hmmh.Api.Factories;
using Hmmh.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hmmh.Api.Tests.Services;

/// <summary>
///     Tests for <see cref="CalorieService" />.
/// </summary>
[TestClass]
public sealed class CalorieServiceTests
{
    /// <summary>
    ///     Ensures invalid date ranges throw validation errors.
    /// </summary>
    [TestMethod]
    public async Task GetCaloriesAsync_ThrowsForInvalidRange()
    {
        // Verify the service rejects invalid ranges.
        var repository = new InMemoryRepository<CalorieEntry>();
        var service = new CalorieService(repository, new CalorieEntryFactory(), new CalorieEntryResponseFactory(), new NullLogger<CalorieService>());

        await Assert.ThrowsExceptionAsync<ValidationApiException>(async () =>
        {
            await service.GetCaloriesAsync(Guid.NewGuid(), new DateOnly(2026, 2, 10), new DateOnly(2026, 2, 9), CancellationToken.None);
        });
    }

    /// <summary>
    ///     Ensures creation trims optional text values.
    /// </summary>
    [TestMethod]
    public async Task CreateCalorieAsync_TrimsOptionalText()
    {
        // Verify optional text is normalized before saving.
        var repository = new InMemoryRepository<CalorieEntry>();
        var service = new CalorieService(repository, new CalorieEntryFactory(), new CalorieEntryResponseFactory(), new NullLogger<CalorieService>());
        var request = new CalorieEntryRequest
        {
            Date = new DateOnly(2026, 2, 10),
            Calories = 500,
            FoodName = "  Salad  ",
            PartOfDay = "  Lunch ",
            Note = "  Fresh  ",
        };

        var response = await service.CreateCalorieAsync(Guid.NewGuid(), request, CancellationToken.None);

        Assert.AreEqual("Salad", response.FoodName);
        Assert.AreEqual("Lunch", response.PartOfDay);
        Assert.AreEqual("Fresh", response.Note);
        Assert.AreEqual(1, repository.Items.Count);
    }

    /// <summary>
    ///     Ensures delete throws when entry is missing.
    /// </summary>
    [TestMethod]
    public async Task DeleteCalorieAsync_ThrowsWhenMissing()
    {
        // Verify missing entries trigger a not found exception.
        var repository = new InMemoryRepository<CalorieEntry>();
        var service = new CalorieService(repository, new CalorieEntryFactory(), new CalorieEntryResponseFactory(), new NullLogger<CalorieService>());

        await Assert.ThrowsExceptionAsync<NotFoundApiException>(async () =>
        {
            await service.DeleteCalorieAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        });
    }
}
