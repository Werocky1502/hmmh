using Hmmh.Api.Data;
using Hmmh.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles weight tracking for authenticated users.
/// </summary>
[ApiController]
[Authorize]
[Route("api/weights")]
public sealed class WeightsController : ControllerBase
{
    private readonly HmmhDbContext dbContext;
    private readonly ILogger<WeightsController> logger;
    private readonly UserManager<ApplicationUser> userManager;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WeightsController" /> class.
    /// </summary>
    /// <param name="dbContext">Database context for weight data.</param>
    /// <param name="userManager">User manager for Identity operations.</param>
    /// <param name="logger">Logger for weight activity.</param>
    public WeightsController(
        HmmhDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger<WeightsController> logger)
    {
        // Capture dependencies required for weight tracking.
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.logger = logger;
    }

    /// <summary>
    ///     Returns a single weight entry for a specific date.
    /// </summary>
    /// <param name="date">Date of the weight entry.</param>
    /// <returns>The matching weight entry.</returns>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(WeightEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WeightEntryResponse>> GetWeightByDate([FromRoute] DateOnly date)
    {
        // Look up the current user and return their weight entry for the requested date.
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new { message = "Unable to locate account." });
        }

        var entry = await dbContext.WeightEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(weight => weight.UserId == user.Id && weight.EntryDate == date);

        if (entry is null)
        {
            return Ok(new WeightEntryResponse
            {
                Id = Guid.Empty,
                Date = date,
                WeightKg = 0,
            });
        }

        return Ok(BuildResponse(entry));
    }

    /// <summary>
    ///     Returns all weight entries for a date range.
    /// </summary>
    /// <param name="startDate">Start of the date range (inclusive).</param>
    /// <param name="endDate">End of the date range (inclusive).</param>
    /// <returns>List of weight entries within the range.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WeightEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<WeightEntryResponse>>> GetWeights(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        // Validate the date range and return all weights for the user.
        if (endDate < startDate)
        {
            return BadRequest(new { message = "End date must be on or after the start date." });
        }

        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new { message = "Unable to locate account." });
        }

        var entries = await dbContext.WeightEntries
            .AsNoTracking()
            .Where(weight => weight.UserId == user.Id)
            .Where(weight => weight.EntryDate >= startDate && weight.EntryDate <= endDate)
            .OrderBy(weight => weight.EntryDate)
            .ToListAsync();

        var response = entries.Select(BuildResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    ///     Creates or updates a weight entry for the supplied date.
    /// </summary>
    /// <param name="request">Weight entry payload.</param>
    /// <returns>The saved weight entry.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(WeightEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WeightEntryResponse>> UpsertWeight([FromBody] WeightEntryRequest request)
    {
        // Validate the incoming payload before writing to the database.
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new { message = "Unable to locate account." });
        }

        var entry = await dbContext.WeightEntries
            .FirstOrDefaultAsync(weight => weight.UserId == user.Id && weight.EntryDate == request.Date);

        if (entry is null)
        {
            entry = new WeightEntry
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                EntryDate = request.Date,
                WeightKg = request.WeightKg,
            };
            dbContext.WeightEntries.Add(entry);
        }
        else
        {
            entry.WeightKg = request.WeightKg;
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Saved weight entry for user {UserId} on {Date}.", user.Id, request.Date);

        return Ok(BuildResponse(entry));
    }

    /// <summary>
    ///     Deletes a weight entry by id.
    /// </summary>
    /// <param name="id">Identifier of the weight entry.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteWeight([FromRoute] Guid id)
    {
        // Remove a weight entry that belongs to the current user.
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized(new { message = "Unable to locate account." });
        }

        var entry = await dbContext.WeightEntries
            .FirstOrDefaultAsync(weight => weight.Id == id && weight.UserId == user.Id);

        if (entry is null)
        {
            return NotFound();
        }

        dbContext.WeightEntries.Remove(entry);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Deleted weight entry {EntryId} for user {UserId}.", id, user.Id);

        return NoContent();
    }

    private static WeightEntryResponse BuildResponse(WeightEntry entry)
    {
        // Map the entity to the API response contract.
        return new WeightEntryResponse
        {
            Id = entry.Id,
            Date = entry.EntryDate,
            WeightKg = entry.WeightKg,
        };
    }

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        // Resolve the current Identity user from the JWT claims.
        return await userManager.GetUserAsync(User);
    }
}
