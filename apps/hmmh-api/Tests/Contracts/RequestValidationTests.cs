using System.ComponentModel.DataAnnotations;
using Hmmh.Api.Contracts.Requests;

namespace Hmmh.Api.Tests.Contracts;

/// <summary>
///     Tests for request contract validation.
/// </summary>
[TestClass]
public sealed class RequestValidationTests
{
    /// <summary>
    ///     Ensures auth requests require minimum lengths.
    /// </summary>
    [TestMethod]
    public void AuthRequest_RequiresMinimumLengths()
    {
        // Verify the auth request enforces minimum length constraints.
        var request = new AuthRequest { Login = "ab", Password = "short" };
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(request, context, results, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(results.Count >= 1);
    }

    /// <summary>
    ///     Ensures weight requests enforce range validation.
    /// </summary>
    [TestMethod]
    public void WeightEntryRequest_EnforcesRange()
    {
        // Verify weight range validation fails for out-of-range values.
        var request = new WeightEntryRequest { Date = new DateOnly(2026, 2, 10), WeightKg = 10 };
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(request, context, results, true);

        Assert.IsFalse(isValid);
        Assert.IsTrue(results.Count >= 1);
    }

    /// <summary>
    ///     Ensures calorie requests validate when required values are supplied.
    /// </summary>
    [TestMethod]
    public void CalorieEntryRequest_ValidPayload_PassesValidation()
    {
        // Verify validation succeeds for a complete payload.
        var request = new CalorieEntryRequest { Date = new DateOnly(2026, 2, 10), Calories = 400 };
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(request, context, results, true);

        Assert.IsTrue(isValid);
        Assert.AreEqual(0, results.Count);
    }
}
