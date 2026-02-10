using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Handles weight tracking for authenticated users.
/// </summary>
[ApiController]
[Authorize]
[Route("api/weights")]
public sealed class WeightsController : ControllerBase
{
    private readonly IWeightService weightService;
    private readonly ICurrentUserAccessor currentUser;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WeightsController" /> class.
    /// </summary>
    /// <param name="weightService">Weight service for business workflows.</param>
    /// <param name="currentUser">Accessor for the current user.</param>
    public WeightsController(
        IWeightService weightService,
        ICurrentUserAccessor currentUser)
    {
        // Capture dependencies required for weight tracking.
        this.weightService = weightService;
        this.currentUser = currentUser;
    }

    /// <summary>
    ///     Returns a single weight entry for a specific date.
    /// </summary>
    /// <param name="date">Date of the weight entry.</param>
    /// <returns>The matching weight entry.</returns>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(WeightEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WeightEntryResponse>> GetWeightByDate(
        [FromRoute] DateOnly date,
        CancellationToken cancellationToken)
    {
        // Delegate query logic to the weight service.
        var response = await weightService.GetWeightByDateAsync(currentUser.UserId, date, cancellationToken);
        return Ok(response);
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
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // Delegate query logic to the weight service.
        var response = await weightService.GetWeightsAsync(currentUser.UserId, startDate, endDate, cancellationToken);
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
    public async Task<ActionResult<WeightEntryResponse>> UpsertWeight(
        [FromBody] WeightEntryRequest request,
        CancellationToken cancellationToken)
    {
        // Delegate upsert logic to the weight service.
        var response = await weightService.UpsertWeightAsync(currentUser.UserId, request, cancellationToken);
        return Ok(response);
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
    public async Task<IActionResult> DeleteWeight(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        // Delegate deletion logic to the weight service.
        await weightService.DeleteWeightAsync(currentUser.UserId, id, cancellationToken);
        return NoContent();
    }
}
