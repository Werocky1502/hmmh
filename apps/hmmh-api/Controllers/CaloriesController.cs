using Hmmh.Api.Data;
using Hmmh.Api.Models;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles calorie tracking for authenticated users.
/// </summary>
[ApiController]
[Authorize]
[Route("api/calories")]
public sealed class CaloriesController : ControllerBase
{
    private readonly HmmhDbContext dbContext;
    private readonly ILogger<CaloriesController> logger;
    private readonly ICurrentUserAccessor currentUser;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CaloriesController" /> class.
    /// </summary>
    /// <param name="dbContext">Database context for calorie data.</param>
    /// <param name="currentUser">Accessor for the current user.</param>
    /// <param name="logger">Logger for calorie activity.</param>
    public CaloriesController(
        HmmhDbContext dbContext,
        ICurrentUserAccessor currentUser,
        ILogger<CaloriesController> logger)
    {
        // Capture dependencies required for calorie tracking.
        this.dbContext = dbContext;
        this.currentUser = currentUser;
        this.logger = logger;
    }

    /// <summary>
    ///     Returns all calorie entries for a specific date.
    /// </summary>
    /// <param name="date">Date of the calorie entries.</param>
    /// <returns>List of calorie entries for the date.</returns>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(IReadOnlyList<CalorieEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CalorieEntryResponse>>> GetCaloriesByDate([FromRoute] DateOnly date)
    {
        // Look up the current user and return their calorie entries for the requested date.
        var entries = await dbContext.CalorieEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == currentUser.UserId && entry.EntryDate == date)
            .OrderBy(entry => entry.Id)
            .ToListAsync();

        var response = entries.Select(BuildResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    ///     Returns all calorie entries for a date range.
    /// </summary>
    /// <param name="startDate">Start of the date range (inclusive).</param>
    /// <param name="endDate">End of the date range (inclusive).</param>
    /// <returns>List of calorie entries within the range.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CalorieEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CalorieEntryResponse>>> GetCalories(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        // Validate the date range and return all calories for the user.
        if (endDate < startDate)
        {
            return BadRequest(new { message = "End date must be on or after the start date." });
        }

        var entries = await dbContext.CalorieEntries
            .AsNoTracking()
            .Where(entry => entry.UserId == currentUser.UserId)
            .Where(entry => entry.EntryDate >= startDate && entry.EntryDate <= endDate)
            .OrderBy(entry => entry.EntryDate)
            .ThenBy(entry => entry.Id)
            .ToListAsync();

        var response = entries.Select(BuildResponse).ToList();
        return Ok(response);
    }

    /// <summary>
    ///     Creates a new calorie entry.
    /// </summary>
    /// <param name="request">Calorie entry payload.</param>
    /// <returns>The saved calorie entry.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CalorieEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CalorieEntryResponse>> CreateCalorie([FromBody] CalorieEntryRequest request)
    {
        // Validate the incoming payload before writing to the database.
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entry = new CalorieEntry
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            EntryDate = request.Date,
            Calories = request.Calories,
            FoodName = NormalizeText(request.FoodName),
            PartOfDay = NormalizeText(request.PartOfDay),
            Note = NormalizeText(request.Note),
        };

        dbContext.CalorieEntries.Add(entry);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Saved calorie entry for user {UserId} on {Date}.", currentUser.UserId, request.Date);

        return Ok(BuildResponse(entry));
    }

    /// <summary>
    ///     Deletes a calorie entry by id.
    /// </summary>
    /// <param name="id">Identifier of the calorie entry.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCalorie([FromRoute] Guid id)
    {
        // Remove a calorie entry that belongs to the current user.
        var entry = await dbContext.CalorieEntries
            .FirstOrDefaultAsync(calorie => calorie.Id == id && calorie.UserId == currentUser.UserId);

        if (entry is null)
        {
            return NotFound();
        }

        dbContext.CalorieEntries.Remove(entry);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Deleted calorie entry {EntryId} for user {UserId}.", id, currentUser.UserId);

        return NoContent();
    }

    private static CalorieEntryResponse BuildResponse(CalorieEntry entry)
    {
        // Map the entity to the API response contract.
        return new CalorieEntryResponse
        {
            Id = entry.Id,
            Date = entry.EntryDate,
            Calories = entry.Calories,
            FoodName = entry.FoodName,
            PartOfDay = entry.PartOfDay,
            Note = entry.Note,
        };
    }

    private static string? NormalizeText(string? value)
    {
        // Normalize optional text input to trimmed values.
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

}
