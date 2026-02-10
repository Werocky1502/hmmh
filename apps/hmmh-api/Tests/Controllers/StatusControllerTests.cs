using Hmmh.Api.Controllers;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Factories;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Tests.Controllers;

/// <summary>
///     Tests for <see cref="StatusController" />.
/// </summary>
[TestClass]
public sealed class StatusControllerTests
{
    /// <summary>
    ///     Ensures the status endpoint returns the factory payload.
    /// </summary>
    [TestMethod]
    public void GetStatus_ReturnsOkResult()
    {
        // Verify the controller returns the expected status response.
        var response = new StatusResponse("hmmh-api", "ok", DateTimeOffset.UtcNow);
        var factory = new StubStatusResponseFactory(response);
        var controller = new StatusController(factory);

        var result = controller.GetStatus();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(response, okResult.Value);
    }

    private sealed class StubStatusResponseFactory : IStatusResponseFactory
    {
        private readonly StatusResponse response;

        public StubStatusResponseFactory(StatusResponse response)
        {
            // Capture the response for test assertions.
            this.response = response;
        }

        public StatusResponse Create(string service, string status, DateTimeOffset timestamp)
        {
            // Return the fixed response for the test.
            return response;
        }
    }
}
