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
    #region OpenApi documentation
        [EndpointSummary("Get all monitored sites")]
        [EndpointDescription("Retrieves all monitored sites.")]

        [ProducesResponseType(typeof(IEnumerable<MonitoredSiteResponse>), StatusCodes.Status200OK)]

        [Produces("application/json")]
    #endregion
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

    #region OpenApi documentation
        [EndpointSummary("Get monitored site by id.")]
        [EndpointDescription("Retrieves a monitored site using its unique identifier.")]

        [ProducesResponseType(typeof(MonitoredSiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        [Produces("application/json")]
    #endregion
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

    #region OpenApi documentation
        [EndpointSummary("Get monitored sites by name.")]
        [EndpointDescription("Retrieves all monitored sites using name of part of monitored site's name")]

        [ProducesResponseType(typeof(IEnumerable<MonitoredSiteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        [Produces("application/json")]
    #endregion
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MonitoredSiteResponse>>> GetMonitoredSitesByName(
        [FromQuery, Required, StringLength(255)] string name,
        CancellationToken cancellationToken)
    {
        var monitoredSites = 
            await siteService.GetMonitoredSitesByName(name, cancellationToken);

        var responses =
            monitoredSites.Select(site => new MonitoredSiteResponse(site));

        return Ok(responses);
    }

    #region OpenApi documentation
        [EndpointSummary("Add a site to monitoring")]
        [EndpointDescription("Adds a site to monitoring. A site name and URL are required.")]

        [ProducesResponseType(typeof(MonitoredSiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

        [Consumes("application/json")]
        [Produces("application/json")]
    #endregion
    [HttpPost]
    public async Task<IActionResult> CreateMonitoredSite(
        [FromBody] CreateMonitoredSiteRequest request,
        CancellationToken cancellationToken)
    {
        var createResult = await siteService.CreateMonitoredSiteAsync(
            request, cancellationToken);

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

    #region OpenApi documentation
        [EndpointSummary("Update monitoring site data.")]
        [EndpointDescription("Updates a monitored site's details while preserving its unique ID.")]

        [ProducesResponseType(typeof(MonitoredSiteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        [Consumes("application/json")]
        [Produces("application/json")]
    #endregion
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

    #region OpenApi documentation
        [EndpointSummary("Deletes a monitored site.")]
        [EndpointDescription("Deletes a monitored site by its unique ID.")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]

        [Produces("application/json")]
    #endregion
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
