using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PulseWatch.Application.Interfaces;
using PulseWatch.Application.Services;

namespace PulseWatch.Api.Controllers;

[ApiController]
[Route("api/monitoring")]
public class MonitoringController(
    SiteMonitoringService monitoringService,
    MonitoredSiteService siteService) : ControllerBase
{
    #region OpenApi documentation
        [EndpointSummary("Check site.")]
        [EndpointDescription("Checks the availability of a page and its server using the specified URL.")]

        [ProducesResponseType(typeof(ISiteCheckResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        [Consumes("application/json")]
        [Produces("application/json")]
    #endregion
    [HttpPost("check-site")]
    public async Task<ActionResult<ISiteCheckResult>> CheckByUrl(
        [FromBody, Required, Url] string url,
        CancellationToken cancellationToken)
    {
        return Ok(
            await monitoringService.CheckByUrlAsync(url, cancellationToken));
    }

    #region OpenApi documentation
        [EndpointSummary("Check a monitored site by ID.")]
        [EndpointDescription("Checks the site by its ID, provided it is registered for monitoring.")]

        [ProducesResponseType(typeof(ISiteCheckResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        [Produces("application/json")]
    #endregion
    [HttpGet("check-by-id/{id:int}")]
    public async Task<Results<Ok<ISiteCheckResult>, NotFound<string>>> CheckById(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var checkResult = await monitoringService.CheckByIdAsync(
            id,
            cancellationToken);

        if (checkResult.IsFailure)
        {
            return TypedResults.NotFound(checkResult.ErrorMessage);
        }

        return TypedResults.Ok(checkResult.Value);
    }

    #region OpenApi documentation
        [EndpointSummary("Check all monitored sites")]

        [ProducesResponseType(typeof(IEnumerable<ISiteCheckResult>), StatusCodes.Status200OK)]

        [Consumes("application/json")]
    #endregion
    [HttpGet("check-all")]
    public async Task<ActionResult<IEnumerable<ISiteCheckResult>>> CheckAll(
        CancellationToken cancellationToken)
    {
        IEnumerable<ISiteCheckResult> sitesCheckResults
            = await monitoringService.CheckAllAsync(cancellationToken);

        return Ok(sitesCheckResults);
    }

    #region OpenApi documentation
        [EndpointSummary("Check sites by name")]
        [EndpointDescription("Check all monitored sites using name of part of monitored site's name")]

        [ProducesResponseType(typeof(IEnumerable<ISiteCheckResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        [Produces("application/json")]
    #endregion
    [HttpGet("check-by-name")]
    public async Task<ActionResult<IEnumerable<ISiteCheckResult>>> CheckByName(
        [FromQuery, Required, StringLength(255)] string name,
        CancellationToken cancellationToken)
    {
        var sites = await siteService.GetMonitoredSitesByName(
            name, cancellationToken);

        var siteCheckResults = await monitoringService.CheckManyAsync(
            sites, cancellationToken);

        return Ok(siteCheckResults);
    }
}
