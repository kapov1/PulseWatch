using PulseWatch.Common.Models;

namespace PulseWatch.Application.Interfaces;

public interface ISiteAvailabilityChecker
{
    Task<Result> CheckAvailabilityAsync(string url, CancellationToken cancellationToken);
}
