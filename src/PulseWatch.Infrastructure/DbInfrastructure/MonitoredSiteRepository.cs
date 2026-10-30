using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PulseWatch.Application;
using PulseWatch.Common.Models;
using PulseWatch.Domain.Models;

namespace PulseWatch.Infrastructure.DbInfrastructure;

public class MonitoredSiteRepository(
    AppDbContext dbContext,
    ILogger<MonitoredSiteRepository> logger) : IMonitoredSiteRepository
{
    private readonly IQueryable<MonitoredSite> monitoredSites = dbContext.MonitoredSites;

    public async Task<Result<MonitoredSite>> CreateAsync(
        MonitoredSite monitoredSite,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.MonitoredSites.AddAsync(monitoredSite, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Monitored site {Id} was created", monitoredSite.Id);

            return Result<MonitoredSite>.Success(monitoredSite);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex, "Failed to create monitored site.");

            return Result<MonitoredSite>.Failure("Failed to save monitored site.");
        }
    }

    public async Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var getSiteResult = 
            await GetByIdAsync(id, cancellationToken);

        if (getSiteResult.IsFailure)
        {
            return Result.Failure(
                getSiteResult.ErrorMessage);
        } 

        var site = getSiteResult.Value;

        dbContext.MonitoredSites.Remove(site);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Monitored site with {Id} id was deleeted", id);

        return Result.Success();
    }

    public async Task<IEnumerable<MonitoredSite>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await monitoredSites.ToListAsync(cancellationToken);
    }

    public async Task<Result<MonitoredSite>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var site = 
            await monitoredSites.FirstOrDefaultAsync(ms => ms.Id == id, cancellationToken);

        if (site is null)
        {
            logger.LogWarning(
                "Monitored site with {Id} id was not found.", id);

            return Result<MonitoredSite>.Failure(
                "Monitored site not found.");
        }

        logger.LogDebug(
                "Monitored site with {Id} id was found.", id);

        return Result<MonitoredSite>.Success(site);
    }

    public async Task<IEnumerable<MonitoredSite>> GetByName(
        string name,
        CancellationToken cancellationToken)
    {
        return await monitoredSites
            .Where(site => site.Name.Contains(name))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<MonitoredSite>> UpdateAsync(
        MonitoredSite newSiteData,
        CancellationToken cancellationToken)
    {
        var result = 
            await GetByIdAsync(newSiteData.Id, cancellationToken);

        if (result.IsFailure)
        {
            return Result<MonitoredSite>.Failure(
                result.ErrorMessage);
        }

        var site = result.Value;

        site.Name = newSiteData.Name;
        site.Url = newSiteData.Url;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug(
            "Monitored site with {Id} id was updated", site.Id);

        return Result<MonitoredSite>.Success(newSiteData);
    }
}
