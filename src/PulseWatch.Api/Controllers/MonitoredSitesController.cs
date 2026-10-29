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
    public async Task<Results<Ok<IEnumerable<MonitoredSite>>, NotFound>> GetAllMonitoredSites()
    {
        var sites = await siteService.GetAllMonitoredSitesAsync(CancellationToken.None);

        if (!sites.Any()) return TypedResults.NotFound();

        return TypedResults.Ok(sites);
    }

    [HttpGet("{id:int}")]
    public async Task<Results<Ok<MonitoredSite>, NotFound<string>>> GetMonitoredSiteById(int id)
    {
        var getSiteResult = await siteService.GetMonitoredSiteByIdAsync(id, CancellationToken.None);

        if (getSiteResult.IsFailure) return TypedResults.NotFound(getSiteResult.ErrorMessage);

        return TypedResults.Ok(getSiteResult.Value);
    }

    [HttpPost("by-name")]
    public async Task<Results<Ok<IEnumerable<MonitoredSite>>, NotFound<string>>> GetMonitoredSiteByName(
        [FromBody, Required, StringLength(255)] string name
    )
    {
        var sites = await siteService.GetMonitoredSitesByName(name, CancellationToken.None);

        if (!sites.Any()) return TypedResults.NotFound("The sites with this name doesn't exist.");

        return TypedResults.Ok(sites);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMonitoredSite(
        [FromBody] CreateMonitoredSiteRequest request)
    {
        var createResult = await siteService.CreateMonitoredSiteAsync(
            request,
            CancellationToken.None);

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
        int id)
    {
        MonitoredSite newSiteData = new(request.Name, request.Url) { Id = id };

        var updateResult = await siteService.UpdateMonitoredSiteAsync(
            newSiteData,
             CancellationToken.None);

        if (updateResult.IsFailure) return TypedResults.NotFound(updateResult.ErrorMessage);

        return TypedResults.Ok(updateResult.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<Results<NoContent, NotFound<string>>> DeleteMonitoredSite(
        [Range(1, int.MaxValue)] int id)
    {
        var deleteResult = await siteService.DeleteMonitoredSiteAsync(id, CancellationToken.None);

        if (deleteResult.IsFailure) return TypedResults.NotFound(deleteResult.ErrorMessage);

        return TypedResults.NoContent();
    }
}
