using Hmmh.Api.Controllers;
using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Tests.Controllers;

/// <summary>
///     Tests for <see cref="AuthController" />.
/// </summary>
[TestClass]
public sealed class AuthControllerTests
{
    /// <summary>
    ///     Ensures sign-up returns the account response.
    /// </summary>
    [TestMethod]
    public async Task SignUp_ReturnsOkResult()
    {
        // Verify the controller returns the account response from the service.
        var request = new AuthRequest { Login = "hannah", Password = "password123" };
        var response = new AccountResponse { UserName = "hannah" };
        var service = new StubAuthService(response);
        var controller = new AuthController(service, new StubCurrentUserAccessor(Guid.NewGuid()));

        var result = await controller.SignUp(request, CancellationToken.None);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreSame(response, okResult.Value);
        Assert.AreSame(request, service.LastRequest);
    }

    /// <summary>
    ///     Ensures delete account delegates to the auth service.
    /// </summary>
    [TestMethod]
    public async Task DeleteAccount_ReturnsNoContent()
    {
        // Verify the controller calls the service with the current user id.
        var userId = Guid.NewGuid();
        var service = new StubAuthService(new AccountResponse());
        var controller = new AuthController(service, new StubCurrentUserAccessor(userId));

        var result = await controller.DeleteAccount(CancellationToken.None);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.AreEqual(userId, service.LastUserId);
    }

    private sealed class StubAuthService : IAuthService
    {
        private readonly AccountResponse response;

        public StubAuthService(AccountResponse response)
        {
            // Capture the response for the test.
            this.response = response;
        }

        public AuthRequest? LastRequest { get; private set; }

        public Guid LastUserId { get; private set; }

        public Task<AccountResponse> SignUpAsync(AuthRequest request, CancellationToken cancellationToken)
        {
            // Record the request and return the configured response.
            LastRequest = request;
            return Task.FromResult(response);
        }

        public Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken)
        {
            // Record the user id for assertions.
            LastUserId = userId;
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
