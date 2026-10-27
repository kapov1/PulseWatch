using PulseWatch.Common.Models;
using PulseWatch.Domain;

namespace PulseWatch.Application;

public interface IMonitoredSiteRepository
{
    Task<IEnumerable<MonitoredSite>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<MonitoredSite>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Result<MonitoredSite>> CreateAsync(MonitoredSite monitoredSite, CancellationToken cancellationToken);

    Task<Result<MonitoredSite>> UpdateAsync(MonitoredSite monitoredSite, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}
