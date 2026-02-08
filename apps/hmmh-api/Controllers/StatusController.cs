using Hmmh.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hmmh.Api.Controllers;

/// <summary>
///     Provides operational status for the HMMH API.
/// </summary>
[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    /// <summary>
    ///     Returns basic service status and timestamp.
    /// </summary>
    /// <returns>Status payload for health checks and diagnostics.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(StatusResponse), StatusCodes.Status200OK)]
    public ActionResult<StatusResponse> GetStatus()
    {
        var response = new StatusResponse("hmmh-api", "ok", DateTimeOffset.UtcNow);
        return Ok(response);
    }
}
