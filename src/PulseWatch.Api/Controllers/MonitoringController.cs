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
    [HttpPost("check-url")]
    public async Task<ActionResult<ISiteCheckResult>> CheckUrl(
        [FromBody, Required, Url] string url,
        CancellationToken cancellationToken)
    {
        return Ok(
            await monitoringService.CheckByUrlAsync(url, cancellationToken));
    }

    [HttpGet("check-by-id/{id:int}")]
    public async Task<Results<Ok<ISiteCheckResult>, NotFound<string>>> CheckUrlById(
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

    [HttpGet("check-all")]
    public async Task<ActionResult<IEnumerable<ISiteCheckResult>>> CheckAll(
        CancellationToken cancellationToken)
    {
        IEnumerable<ISiteCheckResult> sitesCheckResults
            = await monitoringService.CheckAllAsync(cancellationToken);

        return Ok(sitesCheckResults);
    }

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
