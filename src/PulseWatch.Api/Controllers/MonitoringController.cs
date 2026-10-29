using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PulseWatch.Application.Interfaces;
using PulseWatch.Application.Services;

namespace PulseWatch.Api.Controllers;

[ApiController]
[Route("api/monioring")]
public class MonitoringController(SiteMonitoringService monitoringService) : ControllerBase
{
    [HttpPost("check-url")]
    public async Task<ActionResult<ISiteCheckResult>> CheckUrl([FromBody, Required, Url] string url)
    {
        return Ok(await monitoringService.CheckByUrlAsync(url, CancellationToken.None));
    }

    [HttpGet("check-site/{id:int}")]
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
}
