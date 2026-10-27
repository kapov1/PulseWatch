using PulseWatch.Common.Models;

namespace PulseWatch.Application.Interfaces;

public interface ISiteAvailabilityChecker
{
    Task<Result> CheckAsync(string url, CancellationToken cancellationToken);
}
