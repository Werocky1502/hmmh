using Hmmh.Api.Data;
using Hmmh.Api.Models;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles sign-up and account deletion for HMMH users.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> logger;
    private readonly ICurrentUserAccessor currentUser;
    private readonly HmmhDbContext dbContext;
    private readonly IPasswordHasherService passwordHasher;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthController" /> class.
    /// </summary>
    /// <param name="dbContext">Database context for user operations.</param>
    /// <param name="passwordHasher">Password hashing service.</param>
    /// <param name="currentUser">Accessor for the current user.</param>
    /// <param name="logger">Logger for auth events.</param>
    public AuthController(
        HmmhDbContext dbContext,
        IPasswordHasherService passwordHasher,
        ICurrentUserAccessor currentUser,
        ILogger<AuthController> logger)
    {
        // Capture dependencies needed for authentication workflows.
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.currentUser = currentUser;
        this.logger = logger;
    }

    /// <summary>
    ///     Registers a new user with a login and password.
    /// </summary>
    /// <param name="request">Sign-up payload.</param>
    /// <returns>Account details for the new user.</returns>
    [HttpPost("sign-up")]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> SignUp([FromBody] AuthRequest request)
    {
        // Reject duplicate logins early to keep errors explicit.
        var login = NormalizeLogin(request.Login);
        if (string.IsNullOrWhiteSpace(login))
        {
            return BadRequest(new { message = "Login is required." });
        }
        var existingUser = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.UserName == login);
        if (existingUser)
        {
            return Conflict(new { message = "Login already exists." });
        }

        var user = new ApplicationUser
        {
            UserName = login,
            PasswordHash = passwordHasher.HashPassword(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created new user {Login}.", login);
        return Ok(new AccountResponse
        {
            UserName = user.UserName,
        });
    }

    /// <summary>
    ///     Deletes the currently authenticated account.
    /// </summary>
    /// <returns>No content on success.</returns>
    [Authorize]
    [HttpDelete("delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(candidate => candidate.Id == currentUser.UserId);
        if (user is null)
        {
            return NotFound(new { message = "Account does not exists." });
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Deleted account {Login}.", user.UserName);
        return NoContent();
    }

    private static string NormalizeLogin(string login)
    {
        return login.Trim().ToLowerInvariant();
    }

}
