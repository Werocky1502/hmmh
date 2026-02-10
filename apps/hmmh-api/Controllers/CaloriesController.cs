using Hmmh.Api.Contracts;
using Hmmh.Api.Models;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles calorie tracking for authenticated users.
/// </summary>
[ApiController]
[Authorize]
[Route("api/calories")]
public sealed class CaloriesController : ControllerBase
{
    private readonly ICalorieService calorieService;
    private readonly ICurrentUserAccessor currentUser;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CaloriesController" /> class.
    /// </summary>
    /// <param name="calorieService">Calorie service for business workflows.</param>
    /// <param name="currentUser">Accessor for the current user.</param>
    public CaloriesController(
        ICalorieService calorieService,
        ICurrentUserAccessor currentUser)
    {
        // Capture dependencies required for calorie tracking.
        this.calorieService = calorieService;
        this.currentUser = currentUser;
    }

    /// <summary>
    ///     Returns all calorie entries for a specific date.
    /// </summary>
    /// <param name="date">Date of the calorie entries.</param>
    /// <returns>List of calorie entries for the date.</returns>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(IReadOnlyList<CalorieEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CalorieEntryResponse>>> GetCaloriesByDate(
        [FromRoute] DateOnly date,
        CancellationToken cancellationToken)
    {
        // Delegate query logic to the calorie service.
        var response = await calorieService.GetCaloriesByDateAsync(currentUser.UserId, date, cancellationToken);
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
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // Delegate query logic to the calorie service.
        var response = await calorieService.GetCaloriesAsync(currentUser.UserId, startDate, endDate, cancellationToken);
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
    public async Task<ActionResult<CalorieEntryResponse>> CreateCalorie(
        [FromBody] CalorieEntryRequest request,
        CancellationToken cancellationToken)
    {
        // Delegate creation logic to the calorie service.
        var response = await calorieService.CreateCalorieAsync(currentUser.UserId, request, cancellationToken);
        return Ok(response);
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
    public async Task<IActionResult> DeleteCalorie(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        // Delegate deletion logic to the calorie service.
        await calorieService.DeleteCalorieAsync(currentUser.UserId, id, cancellationToken);
        return NoContent();
    }
}
