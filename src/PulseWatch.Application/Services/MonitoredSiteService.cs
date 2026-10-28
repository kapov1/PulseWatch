using PulseWatch.Application.Models;
using PulseWatch.Common.Models;
using PulseWatch.Domain;

namespace PulseWatch.Application.Services;

public class MonitoredSiteService(IMonitoredSiteRepository siteRepository)
{
    public async Task<Result<MonitoredSite>> CreateMonitoredSiteAsync(CreateMonitoredSiteRequest request, CancellationToken cancellationToken)
    {
        MonitoredSite site = new(request.Name, request.Url);

        return await siteRepository.CreateAsync(site, cancellationToken);
    }

    public async Task<Result<MonitoredSite>> GetMonitoredSiteByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await siteRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<MonitoredSite>> GetAllMonitoredSitesAsync(CancellationToken cancellationToken)
    {
        return await siteRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Result<MonitoredSite>> UpdateMonitoredSiteAsync(MonitoredSite monitoredSite, CancellationToken cancellationToken)
    {
        return await siteRepository.UpdateAsync(monitoredSite, cancellationToken);
    }

    public async Task<Result> DeleteMonitoredSiteAsync(int id, CancellationToken cancellationToken)
    {
        return await siteRepository.DeleteAsync(id, cancellationToken);
    }
}
