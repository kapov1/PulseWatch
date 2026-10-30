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
    public async Task<ActionResult<IEnumerable<MonitoredSiteResponse>>> GetAllMonitoredSites(
        CancellationToken cancellationToken)
    {
        var monitoredSites =
            await siteService.GetAllMonitoredSitesAsync(cancellationToken);

        var responses =
            monitoredSites.Select(site => new MonitoredSiteResponse(site));

        return Ok(responses);
    }

    [HttpGet("{id:int}")]
    public async Task<Results<Ok<MonitoredSiteResponse>, NotFound<string>>> GetMonitoredSiteById(
        [Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var getSiteResult = await siteService.GetMonitoredSiteByIdAsync(
            id, cancellationToken);

        if (getSiteResult.IsFailure)
        {
            return TypedResults.NotFound(getSiteResult.ErrorMessage);
        } 

        var monitoredSite = getSiteResult.Value;
        MonitoredSiteResponse response = new(monitoredSite);

        return TypedResults.Ok(response);
    }

    [HttpPost("by-name")]
    public async Task<ActionResult<IEnumerable<MonitoredSiteResponse>>> GetMonitoredSiteByName(
        [FromBody, Required, StringLength(255)] string name,
        CancellationToken cancellationToken)
    {
        var monitoredSites = 
            await siteService.GetMonitoredSitesByName(name, cancellationToken);

        var responses =
            monitoredSites.Select(site => new MonitoredSiteResponse(site));

        return Ok(responses);
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
    public async Task<Results<Ok<MonitoredSiteResponse>, NotFound<string>>> UpdateMonitoredSite(
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

        var monitoredSite = updateResult.Value;
        MonitoredSiteResponse response = new(monitoredSite);

        return TypedResults.Ok(response);
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
