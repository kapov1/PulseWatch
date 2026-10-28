using PulseWatch.Application;
using PulseWatch.Application.Interfaces;
using PulseWatch.Common.Models;
using PulseWatch.Domain;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteMonitoringService(HttpClient httpClient, IMonitoredSiteRepository siteRepository)
    : ISiteMonitoringService
{
    private static readonly SemaphoreSlim semaphore = new(10, 10);

    private async Task<ISiteCheckResult> CheckWithSemaphoreAsync(string url, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            return await CheckByUrlAsync(url, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    public async Task<IEnumerable<ISiteCheckResult>> CheckAllAsync(CancellationToken cancellationToken)
    {
        IEnumerable<MonitoredSite> sites = await siteRepository.GetAllAsync(cancellationToken);

        List<Task<ISiteCheckResult>> checkTasks = [];

        foreach (var site in sites)
        {
           checkTasks.Add(CheckWithSemaphoreAsync(site.Url, cancellationToken));
        }

        return await Task.WhenAll(checkTasks);
    }

    public async Task<ISiteCheckResult> CheckByUrlAsync(string url, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = new();

        try
        {
            stopwatch.Start();
            using var response = await httpClient.GetAsync(url, cancellationToken);
            stopwatch.Stop();

            return SiteCheckResult.FromHttpResponse(response, stopwatch.Elapsed.TotalSeconds);
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is SocketException socketException &&
                socketException.SocketErrorCode == SocketError.HostNotFound)
            {
                return SiteCheckResult.FromHostNotFound(stopwatch.Elapsed.TotalSeconds);
            }
        }
        finally
        {
            stopwatch.Stop();
        }

        return SiteCheckResult.FromHttpRequestFailure(stopwatch.Elapsed.TotalSeconds);
    }

    public async Task<Result<ISiteCheckResult>> CheckByIdAsync(int id, CancellationToken cancellationToken)
    {
        var siteGetResult = await siteRepository.GetByIdAsync(id, cancellationToken);

        if (siteGetResult.IsFailure)
        {
            return Result<ISiteCheckResult>.Failure(siteGetResult.ErrorMessage);
        }

        var site = siteGetResult.Value;

        return Result<ISiteCheckResult>.Success(
            await CheckByUrlAsync(site.Url, cancellationToken)
        );
    }
}
