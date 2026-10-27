using PulseWatch.Application.Interfaces;
using PulseWatch.Common.Models;
using PulseWatch.Domain;
using PulseWatch.Infrastructure.DbInfrastructure;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Sockets;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteMonitoringService(HttpClient httpClient, MonitoredSiteRepository siteRepository)
    : ISiteAvailabilityChecker, ISiteMonitoringService
{
    public async Task<IEnumerable<ISiteCheckResult>> CheckAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<MonitoredSite> sites = await siteRepository.GetAllAsync(cancellationToken);

        List<Task<ISiteCheckResult>> checkTasks = [];

        foreach (var site in sites)
        {
            checkTasks.Add(CheckByUrlAsync(site.Url, cancellationToken));
        }

        return await Task.WhenAll(checkTasks);
    }

    public async Task<Result> CheckAvailabilityAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(url, cancellationToken);
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is SocketException socketException &&
                socketException.SocketErrorCode == SocketError.HostNotFound)
            {
                return Result.Failure("The Domain name could not be found.");
            }
        }

        return Result.Failure("The HTTP request failed.");
    }

    public Task<Result<ISiteCheckResult>> CheckByIdAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<ISiteCheckResult> CheckByUrlAsync([Url] string url, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = new();

        stopwatch.Start();
        var checkAvailabilityResult = await CheckAvailabilityAsync(url, cancellationToken);
        stopwatch.Stop();

        if (checkAvailabilityResult.IsFailure)
        {
            return SiteCheckResult.FromHostNotFound(stopwatch.Elapsed.Seconds);
        }

        stopwatch.Restart();
        using var response = await httpClient.GetAsync(url, cancellationToken);
        stopwatch.Stop();

        return SiteCheckResult.FromHttpResponse(response, stopwatch.Elapsed.Seconds);
    }

}
