using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Factories;

namespace Hmmh.Api.Tests.Factories;

/// <summary>
///     Tests for response factories.
/// </summary>
[TestClass]
public sealed class ResponseFactoriesTests
{
    /// <summary>
    ///     Ensures account responses map user data.
    /// </summary>
    [TestMethod]
    public void AccountResponseFactory_MapsUserName()
    {
        // Verify account response mapping.
        var factory = new AccountResponseFactory();
        var user = new ApplicationUser { UserName = "hannah" };

        var response = factory.Create(user);

        Assert.AreEqual("hannah", response.UserName);
    }

    /// <summary>
    ///     Ensures weight response mapping works.
    /// </summary>
    [TestMethod]
    public void WeightEntryResponseFactory_MapsEntry()
    {
        // Verify weight response mapping.
        var factory = new WeightEntryResponseFactory();
        var entry = new WeightEntry
        {
            Id = Guid.NewGuid(),
            EntryDate = new DateOnly(2026, 2, 10),
            WeightKg = 82.1m,
        };

        var response = factory.Create(entry);

        Assert.AreEqual(entry.Id, response.Id);
        Assert.AreEqual(entry.EntryDate, response.Date);
        Assert.AreEqual(entry.WeightKg, response.WeightKg);
    }

    /// <summary>
    ///     Ensures empty weight responses use default values.
    /// </summary>
    [TestMethod]
    public void WeightEntryResponseFactory_CreatesEmptyEntry()
    {
        // Verify empty response uses zeroed values.
        var factory = new WeightEntryResponseFactory();
        var date = new DateOnly(2026, 2, 10);

        var response = factory.CreateEmpty(date);

        Assert.AreEqual(Guid.Empty, response.Id);
        Assert.AreEqual(date, response.Date);
        Assert.AreEqual(0, response.WeightKg);
    }

    /// <summary>
    ///     Ensures calorie response mapping works.
    /// </summary>
    [TestMethod]
    public void CalorieEntryResponseFactory_MapsEntry()
    {
        // Verify calorie response mapping.
        var factory = new CalorieEntryResponseFactory();
        var entry = new CalorieEntry
        {
            Id = Guid.NewGuid(),
            EntryDate = new DateOnly(2026, 2, 10),
            Calories = 650,
            FoodName = "Pasta",
            PartOfDay = "Dinner",
            Note = "Hearty",
        };

        var response = factory.Create(entry);

        Assert.AreEqual(entry.Id, response.Id);
        Assert.AreEqual(entry.EntryDate, response.Date);
        Assert.AreEqual(entry.Calories, response.Calories);
        Assert.AreEqual(entry.FoodName, response.FoodName);
        Assert.AreEqual(entry.PartOfDay, response.PartOfDay);
        Assert.AreEqual(entry.Note, response.Note);
    }

    /// <summary>
    ///     Ensures status responses are built correctly.
    /// </summary>
    [TestMethod]
    public void StatusResponseFactory_CreatesResponse()
    {
        // Verify status response factory output.
        var factory = new StatusResponseFactory();
        var timestamp = DateTimeOffset.UtcNow;

        var response = factory.Create("hmmh-api", "ok", timestamp);

        Assert.AreEqual("hmmh-api", response.Service);
        Assert.AreEqual("ok", response.Status);
        Assert.AreEqual(timestamp, response.Timestamp);
    }
}
