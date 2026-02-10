using Hmmh.Api.Contracts;
using Hmmh.Api.Models;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles sign-up and account deletion for HMMH users.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly ICurrentUserAccessor currentUser;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthController" /> class.
    /// </summary>
    /// <param name="authService">Auth service for account workflows.</param>
    /// <param name="currentUser">Accessor for the current user.</param>
    public AuthController(
        IAuthService authService,
        ICurrentUserAccessor currentUser)
    {
        // Capture dependencies needed for authentication workflows.
        this.authService = authService;
        this.currentUser = currentUser;
    }

    /// <summary>
    ///     Registers a new user with a login and password.
    /// </summary>
    /// <param name="request">Sign-up payload.</param>
    /// <returns>Account details for the new user.</returns>
    [HttpPost("sign-up")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> SignUp(
        [FromBody] AuthRequest request,
        CancellationToken cancellationToken)
    {
        // Delegate sign-up logic to the auth service.
        var response = await authService.SignUpAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    ///     Deletes the currently authenticated account.
    /// </summary>
    /// <returns>No content on success.</returns>
    [Authorize]
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        // Delegate account deletion to the auth service.
        await authService.DeleteAccountAsync(currentUser.UserId, cancellationToken);
        return NoContent();
    }
}
