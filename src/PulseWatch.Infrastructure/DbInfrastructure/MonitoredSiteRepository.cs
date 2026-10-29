using Microsoft.EntityFrameworkCore;
using PulseWatch.Application;
using PulseWatch.Common.Models;
using PulseWatch.Domain.Models;

namespace PulseWatch.Infrastructure.DbInfrastructure;

public class MonitoredSiteRepository(AppDbContext dbContext) : IMonitoredSiteRepository
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

            return Result<MonitoredSite>.Success(monitoredSite);
        }
        catch (DbUpdateException)
        {
            return Result<MonitoredSite>.Failure("Failed to save monitored site.");
        }
    }

    public async Task<Result> DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var site = await monitoredSites.FirstOrDefaultAsync(site => site.Id == id, cancellationToken);

        if (site is null) return Result.Failure("Monitored site not found.");

        dbContext.MonitoredSites.Remove(site);

        await dbContext.SaveChangesAsync(cancellationToken);

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
        var site = await monitoredSites.FirstOrDefaultAsync(ms => ms.Id == id, cancellationToken);

        if (site is null) return Result<MonitoredSite>.Failure("Monitored site not found.");

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
        var result = await GetByIdAsync(newSiteData.Id, cancellationToken);

        if (result.IsFailure) return Result<MonitoredSite>.Failure(result.ErrorMessage);

        var site = result.Value;

        site.Name = newSiteData.Name;
        site.Url = newSiteData.Url;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<MonitoredSite>.Success(newSiteData);
    }
}
