using Hmmh.Api.Db.Models;
using Hmmh.Api.Factories;

namespace Hmmh.Api.Tests.Factories;

/// <summary>
///     Tests for entity factories.
/// </summary>
[TestClass]
public sealed class EntityFactoriesTests
{
    /// <summary>
    ///     Ensures application users are built with expected values.
    /// </summary>
    [TestMethod]
    public void ApplicationUserFactory_CreatesUser()
    {
        // Verify user factory assigns the provided values.
        var factory = new ApplicationUserFactory();

        var user = factory.Create("user", "hash");

        Assert.AreEqual("user", user.UserName);
        Assert.AreEqual("hash", user.PasswordHash);
        Assert.AreNotEqual(Guid.Empty, user.Id);
    }

    /// <summary>
    ///     Ensures weight entries are built with expected values.
    /// </summary>
    [TestMethod]
    public void WeightEntryFactory_CreatesEntry()
    {
        // Verify weight entry factory assigns the provided values.
        var factory = new WeightEntryFactory();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 2, 10);

        var entry = factory.Create(userId, date, 78.3m);

        Assert.AreEqual(userId, entry.UserId);
        Assert.AreEqual(date, entry.EntryDate);
        Assert.AreEqual(78.3m, entry.WeightKg);
        Assert.AreNotEqual(Guid.Empty, entry.Id);
    }

    /// <summary>
    ///     Ensures calorie entries are built with expected values.
    /// </summary>
    [TestMethod]
    public void CalorieEntryFactory_CreatesEntry()
    {
        // Verify calorie entry factory assigns the provided values.
        var factory = new CalorieEntryFactory();
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 2, 10);

        var entry = factory.Create(userId, date, 600, "Soup", "Dinner", "Note");

        Assert.AreEqual(userId, entry.UserId);
        Assert.AreEqual(date, entry.EntryDate);
        Assert.AreEqual(600, entry.Calories);
        Assert.AreEqual("Soup", entry.FoodName);
        Assert.AreEqual("Dinner", entry.PartOfDay);
        Assert.AreEqual("Note", entry.Note);
        Assert.AreNotEqual(Guid.Empty, entry.Id);
    }
}
