using Hmmh.Api.Controllers;
using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Tests.Controllers;

/// <summary>
///     Tests for <see cref="CaloriesController" />.
/// </summary>
[TestClass]
public sealed class CaloriesControllerTests
{
    /// <summary>
    ///     Ensures calorie lookups return the service response.
    /// </summary>
    [TestMethod]
    public async Task GetCaloriesByDate_ReturnsOkResult()
    {
        // Verify the controller returns the calorie response list.
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 2, 10);
        var response = new List<CalorieEntryResponse>
        {
            new CalorieEntryResponse { Id = Guid.NewGuid(), Date = date, Calories = 450 },
        };
        var service = new StubCalorieService { CaloriesByDateResponse = response };
        var controller = new CaloriesController(service, new StubCurrentUserAccessor(userId));

        var result = await controller.GetCaloriesByDate(date, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(response, okResult.Value);
        Assert.AreEqual(userId, service.LastUserId);
        Assert.AreEqual(date, service.LastDate);
    }

    /// <summary>
    ///     Ensures delete requests delegate to the calorie service.
    /// </summary>
    [TestMethod]
    public async Task DeleteCalorie_ReturnsNoContent()
    {
        // Verify the controller calls the delete workflow on the service.
        var userId = Guid.NewGuid();
        var entryId = Guid.NewGuid();
        var service = new StubCalorieService();
        var controller = new CaloriesController(service, new StubCurrentUserAccessor(userId));

        var result = await controller.DeleteCalorie(entryId, CancellationToken.None);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.AreEqual(userId, service.LastUserId);
        Assert.AreEqual(entryId, service.LastEntryId);
    }

    private sealed class StubCalorieService : ICalorieService
    {
        public Guid LastUserId { get; private set; }

        public DateOnly LastDate { get; private set; }

        public Guid LastEntryId { get; private set; }

        public IReadOnlyList<CalorieEntryResponse> CaloriesByDateResponse { get; set; } = Array.Empty<CalorieEntryResponse>();

        public Task<IReadOnlyList<CalorieEntryResponse>> GetCaloriesByDateAsync(
            Guid userId,
            DateOnly date,
            CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            LastDate = date;
            return Task.FromResult(CaloriesByDateResponse);
        }

        public Task<IReadOnlyList<CalorieEntryResponse>> GetCaloriesAsync(
            Guid userId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            return Task.FromResult<IReadOnlyList<CalorieEntryResponse>>(Array.Empty<CalorieEntryResponse>());
        }

        public Task<CalorieEntryResponse> CreateCalorieAsync(
            Guid userId,
            CalorieEntryRequest request,
            CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            return Task.FromResult(new CalorieEntryResponse());
        }

        public Task DeleteCalorieAsync(Guid userId, Guid entryId, CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            LastEntryId = entryId;
            return Task.CompletedTask;
        }
    }

    private sealed class StubCurrentUserAccessor : ICurrentUserAccessor
    {
        public StubCurrentUserAccessor(Guid userId)
        {
            // Capture the user id for controller dependencies.
            UserId = userId;
        }

        public Guid UserId { get; }

        public string? UserName => null;

        public bool IsAuthenticated => true;
    }
}
