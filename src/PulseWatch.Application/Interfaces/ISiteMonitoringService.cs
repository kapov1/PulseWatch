using PulseWatch.Common.Models;

namespace PulseWatch.Application.Interfaces;

public interface ISiteMonitoringService
{
    Task<ISiteCheckResult> CheckByUrlAsync(string url, CancellationToken cancellationToken);

    Task<Result<ISiteCheckResult>> CheckByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<ISiteCheckResult>> CheckAllAsync(CancellationToken cancellationToken);
}
