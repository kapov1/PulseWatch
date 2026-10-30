using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PulseWatch.Application.Models;
using PulseWatch.Application.Services;
using PulseWatch.Domain.Models;

namespace PulseWatch.Api.Controllers;

[ApiController]
[Route("api/sites")]
public class MonitoredSitesController(MonitoredSiteService siteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonitoredSite>>> GetAllMonitoredSites(
        CancellationToken cancellationToken)
    {
        IEnumerable<MonitoredSite> sites =
            await siteService.GetAllMonitoredSitesAsync(cancellationToken);

        return Ok(sites);
    }

    [HttpGet("{id:int}")]
    public async Task<Results<Ok<MonitoredSite>, NotFound<string>>> GetMonitoredSiteById(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var getSiteResult = await siteService.GetMonitoredSiteByIdAsync(
            id,
            cancellationToken);

        if (getSiteResult.IsFailure) return TypedResults.NotFound(getSiteResult.ErrorMessage);

        return TypedResults.Ok(getSiteResult.Value);
    }

    [HttpPost("by-name")]
    public async Task<Results<Ok<IEnumerable<MonitoredSite>>, NotFound<string>>> GetMonitoredSiteByName(
        [FromBody, Required, StringLength(255)] string name,
        CancellationToken cancellationToken)
    {
        var sites = await siteService.GetMonitoredSitesByName(name, cancellationToken);

        return TypedResults.Ok(sites);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMonitoredSite(
        [FromBody] CreateMonitoredSiteRequest request,
        CancellationToken cancellationToken)
    {
        var createResult = await siteService.CreateMonitoredSiteAsync(
            request,
            cancellationToken);

        if (createResult.IsFailure)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                createResult.ErrorMessage);
        }

        MonitoredSiteResponse response = new(createResult.Value);

        return CreatedAtAction(
            nameof(GetMonitoredSiteById),
            new { id = response.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<Results<Ok<MonitoredSite>, NotFound<string>>> UpdateMonitoredSite(
        [FromBody] CreateMonitoredSiteRequest request,
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        MonitoredSite newSiteData = new(request.Name, request.Url) { Id = id };

        var updateResult = await siteService.UpdateMonitoredSiteAsync(
            newSiteData,
            cancellationToken);

        if (updateResult.IsFailure)
        {
            return TypedResults.NotFound(updateResult.ErrorMessage);
        }

        return TypedResults.Ok(updateResult.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<Results<NoContent, NotFound<string>>> DeleteMonitoredSite(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var deleteResult = await siteService.DeleteMonitoredSiteAsync(
            id,
            cancellationToken);

        if (deleteResult.IsFailure)
        {
            return TypedResults.NotFound(deleteResult.ErrorMessage);
        }

        return TypedResults.NoContent();
    }
}
