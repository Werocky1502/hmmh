using Hmmh.Api.Contracts;
using Hmmh.Api.Factories;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Provides operational status for the HMMH API.
/// </summary>
[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    private readonly IStatusResponseFactory responseFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StatusController" /> class.
    /// </summary>
    /// <param name="responseFactory">Factory for status responses.</param>
    public StatusController(IStatusResponseFactory responseFactory)
    {
        this.responseFactory = responseFactory;
    }

    /// <summary>
    ///     Returns basic service status and timestamp.
    /// </summary>
    /// <returns>Status payload for health checks and diagnostics.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    public ActionResult<StatusResponse> GetStatus()
    {
        var response = responseFactory.Create("hmmh-api", "ok", DateTimeOffset.UtcNow);
        return Ok(response);
    }
}
