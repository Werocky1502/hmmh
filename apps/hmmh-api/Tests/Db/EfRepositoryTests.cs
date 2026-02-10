using Hmmh.Api.Db.Data;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Db.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hmmh.Api.Tests.Db;

/// <summary>
///     Tests for <see cref="EfRepository{T}" />.
/// </summary>
[TestClass]
public sealed class EfRepositoryTests
{
    /// <summary>
    ///     Ensures entities can be added and retrieved.
    /// </summary>
    [TestMethod]
    public async Task AddAndFindAsync_ReturnsEntity()
    {
        // Verify repository operations using an in-memory database.
        var options = new DbContextOptionsBuilder<HmmhDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        await using var context = new HmmhDbContext(options);
        var repository = new EfRepository<WeightEntry>(context);
        var entry = new WeightEntry
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            EntryDate = new DateOnly(2026, 2, 10),
            WeightKg = 81.4m,
        };

        await repository.AddAsync(entry, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var result = await repository.FindAsync(weight => weight.Id == entry.Id, false, CancellationToken.None);

        Assert.IsNotNull(result);
        Assert.AreEqual(entry.Id, result.Id);
    }
}
