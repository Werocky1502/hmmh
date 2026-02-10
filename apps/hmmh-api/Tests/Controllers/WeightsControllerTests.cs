using Hmmh.Api.Controllers;
using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Tests.Controllers;

/// <summary>
///     Tests for <see cref="WeightsController" />.
/// </summary>
[TestClass]
public sealed class WeightsControllerTests
{
    /// <summary>
    ///     Ensures weight lookups return the service response.
    /// </summary>
    [TestMethod]
    public async Task GetWeightByDate_ReturnsOkResult()
    {
        // Verify the controller returns the weight response.
        var userId = Guid.NewGuid();
        var date = new DateOnly(2026, 2, 10);
        var response = new WeightEntryResponse { Id = Guid.NewGuid(), Date = date, WeightKg = 82.5m };
        var service = new StubWeightService { WeightByDateResponse = response };
        var controller = new WeightsController(service, new StubCurrentUserAccessor(userId));

        var result = await controller.GetWeightByDate(date, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(response, okResult.Value);
        Assert.AreEqual(userId, service.LastUserId);
        Assert.AreEqual(date, service.LastDate);
    }

    /// <summary>
    ///     Ensures delete requests delegate to the weight service.
    /// </summary>
    [TestMethod]
    public async Task DeleteWeight_ReturnsNoContent()
    {
        // Verify the controller calls the delete workflow on the service.
        var userId = Guid.NewGuid();
        var entryId = Guid.NewGuid();
        var service = new StubWeightService();
        var controller = new WeightsController(service, new StubCurrentUserAccessor(userId));

        var result = await controller.DeleteWeight(entryId, CancellationToken.None);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.AreEqual(userId, service.LastUserId);
        Assert.AreEqual(entryId, service.LastEntryId);
    }

    private sealed class StubWeightService : IWeightService
    {
        public Guid LastUserId { get; private set; }

        public DateOnly LastDate { get; private set; }

        public Guid LastEntryId { get; private set; }

        public WeightEntryResponse WeightByDateResponse { get; set; } = new WeightEntryResponse();

        public Task<WeightEntryResponse> GetWeightByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            LastDate = date;
            return Task.FromResult(WeightByDateResponse);
        }

        public Task<IReadOnlyList<WeightEntryResponse>> GetWeightsAsync(
            Guid userId,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            return Task.FromResult<IReadOnlyList<WeightEntryResponse>>(Array.Empty<WeightEntryResponse>());
        }

        public Task<WeightEntryResponse> UpsertWeightAsync(
            Guid userId,
            WeightEntryRequest request,
            CancellationToken cancellationToken)
        {
            // Record the parameters for assertion.
            LastUserId = userId;
            return Task.FromResult(new WeightEntryResponse());
        }

        public Task DeleteWeightAsync(Guid userId, Guid entryId, CancellationToken cancellationToken)
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
