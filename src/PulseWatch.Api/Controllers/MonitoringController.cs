using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PulseWatch.Application.Interfaces;
using PulseWatch.Application.Services;

namespace PulseWatch.Api.Controllers;

[ApiController]
[Route("api/monioring")]
public class MonitoringController(
    SiteMonitoringService monitoringService,
    MonitoredSiteService siteService) : ControllerBase
{
    [HttpPost("check-url")]
    public async Task<ActionResult<ISiteCheckResult>> CheckUrl([FromBody, Required, Url] string url)
    {
        return Ok(await monitoringService.CheckByUrlAsync(url, CancellationToken.None));
    }

    [HttpGet("check-by-id/{id:int}")]
    public async Task<Results<Ok<ISiteCheckResult>, NotFound<string>>> CheckUrlById(
        [Range(1, int.MaxValue)] int id)
    {
        var checkResult = await monitoringService.CheckByIdAsync(id, CancellationToken.None);

        if (checkResult.IsFailure) return TypedResults.NotFound(checkResult.ErrorMessage);

        return TypedResults.Ok(checkResult.Value);
    }

    [HttpGet("check-all")]
    public async Task<ActionResult<IEnumerable<ISiteCheckResult>>> CheckAll()
    {
        IEnumerable<ISiteCheckResult> sitesCheckResults 
            = await monitoringService.CheckAllAsync(CancellationToken.None);

        return Ok(sitesCheckResults);
    }

    [HttpPost("check-by-name")]
    public async Task<Results<Ok<IEnumerable<ISiteCheckResult>>, NotFound<string>>> CheckByName(
        [FromBody, Required, StringLength(255)] string name)
    {
        var sites = await siteService.GetMonitoredSitesByName
            (name, CancellationToken.None);

        if (!sites.Any()) return TypedResults.NotFound("The site with this name doesn't exist.");

        return TypedResults.Ok(await monitoringService.CheckManyAsync
            (sites, CancellationToken.None));
    }
}
