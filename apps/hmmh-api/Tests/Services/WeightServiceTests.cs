using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Exceptions;
using Hmmh.Api.Factories;
using Hmmh.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Hmmh.Api.Tests.Services;

/// <summary>
///     Tests for <see cref="WeightService" />.
/// </summary>
[TestClass]
public sealed class WeightServiceTests
{
    /// <summary>
    ///     Ensures invalid date ranges throw validation errors.
    /// </summary>
    [TestMethod]
    public async Task GetWeightsAsync_ThrowsForInvalidRange()
    {
        // Verify the service rejects invalid ranges.
        var repository = new InMemoryRepository<WeightEntry>();
        var service = new WeightService(repository, new WeightEntryFactory(), new WeightEntryResponseFactory(), new NullLogger<WeightService>());

        await Assert.ThrowsExceptionAsync<ValidationApiException>(async () =>
        {
            await service.GetWeightsAsync(Guid.NewGuid(), new DateOnly(2026, 2, 10), new DateOnly(2026, 2, 9), CancellationToken.None);
        });
    }

    /// <summary>
    ///     Ensures upserts create new entries when missing.
    /// </summary>
    [TestMethod]
    public async Task UpsertWeightAsync_CreatesEntryWhenMissing()
    {
        // Verify a new entry is created and returned.
        var repository = new InMemoryRepository<WeightEntry>();
        var service = new WeightService(repository, new WeightEntryFactory(), new WeightEntryResponseFactory(), new NullLogger<WeightService>());
        var userId = Guid.NewGuid();
        var request = new WeightEntryRequest { Date = new DateOnly(2026, 2, 10), WeightKg = 80.2m };

        var response = await service.UpsertWeightAsync(userId, request, CancellationToken.None);

        Assert.AreEqual(request.Date, response.Date);
        Assert.AreEqual(request.WeightKg, response.WeightKg);
        Assert.AreEqual(1, repository.Items.Count);
    }

    /// <summary>
    ///     Ensures delete throws when entry is missing.
    /// </summary>
    [TestMethod]
    public async Task DeleteWeightAsync_ThrowsWhenMissing()
    {
        // Verify missing entries trigger a not found exception.
        var repository = new InMemoryRepository<WeightEntry>();
        var service = new WeightService(repository, new WeightEntryFactory(), new WeightEntryResponseFactory(), new NullLogger<WeightService>());

        await Assert.ThrowsExceptionAsync<NotFoundApiException>(async () =>
        {
            await service.DeleteWeightAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        });
    }
}
