using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hmmh.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles sign-in, sign-up, and account deletion for HMMH users.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtSettings jwtSettings;
    private readonly ILogger<AuthController> logger;
    private readonly UserManager<ApplicationUser> userManager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthController" /> class.
    /// </summary>
    /// <param name="userManager">User manager for Identity operations.</param>
    /// <param name="jwtOptions">JWT configuration options.</param>
    /// <param name="logger">Logger for auth events.</param>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthController> logger)
    {
        // Capture dependencies needed for authentication workflows.
        this.userManager = userManager;
        this.jwtSettings = jwtOptions.Value;
        this.logger = logger;
    }

    /// <summary>
    ///     Registers a new user with a login and password.
    /// </summary>
    /// <param name="request">Sign-up payload.</param>
    /// <returns>JWT token and the login name.</returns>
    [HttpPost("sign-up")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> SignUp([FromBody] AuthRequest request)
    {
        // Reject duplicate logins early to keep errors explicit.
        var existingUser = await userManager.FindByNameAsync(request.Login);
        if (existingUser is not null)
        {
            return Conflict(new { message = "Login already exists." });
        }

        var user = new ApplicationUser { UserName = request.Login };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(error => error.Description));
            return BadRequest(new { message });
        }

        logger.LogInformation("Created new user {Login}.", request.Login);
        return Ok(BuildAuthResponse(user));
    }

    /// <summary>
    ///     Signs in an existing user with a login and password.
    /// </summary>
    /// <param name="request">Sign-in payload.</param>
    /// <returns>JWT token and the login name.</returns>
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> SignIn([FromBody] AuthRequest request)
    {
        // Validate the supplied login and password.
        var user = await userManager.FindByNameAsync(request.Login);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid login or password." });
        }

        var isValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid login or password." });
        }

        logger.LogInformation("User {Login} signed in.", request.Login);
        return Ok(BuildAuthResponse(user));
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
        // Pull the current user from the JWT claims.
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized(new { message = "Unable to locate account." });
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(error => error.Description));
            return BadRequest(new { message });
        }

        logger.LogInformation("Deleted account {Login}.", user.UserName);
        return NoContent();
    }

    private AuthResponse BuildAuthResponse(ApplicationUser user)
    {
        // Build a signed JWT token for the authenticated user.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.TokenMinutes),
            signingCredentials: credentials);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserName = user.UserName ?? string.Empty,
        };
    }
}
