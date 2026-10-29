using PulseWatch.Application.Interfaces;
using System.Diagnostics;
using System.Net.Sockets;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class HttpSiteChecker(HttpClient httpClient) : IHttpSiteChecker
{
    private static readonly SemaphoreSlim semaphore = new(10, 10);

    private async Task<ISiteCheckResult> CheckWithSemaphoreAsync(
        string url,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            return await CheckSiteAsync(url, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<IEnumerable<ISiteCheckResult>> CheckManyAsync(
        IEnumerable<string> urls,
        CancellationToken cancellationToken)
    {
        List<Task<ISiteCheckResult>> checkTasks = [];

        foreach (var url in urls)
        {
            checkTasks.Add(CheckWithSemaphoreAsync(url, cancellationToken));
        }

        return await Task.WhenAll(checkTasks);
    }

    public async Task<ISiteCheckResult> CheckSiteAsync(
        string url,
        CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = new();

        try
        {
            stopwatch.Start();
            using var response = await httpClient.GetAsync(url, cancellationToken);

            return SiteCheckResult.FromHttpResponse(
                url,
                response,
                stopwatch.Elapsed.TotalSeconds);
        }
        catch (HttpRequestException ex) when (
            ex.InnerException is SocketException socketException &&
            socketException.SocketErrorCode == SocketError.HostNotFound)
        {
            return SiteCheckResult.FromHostNotFound(url, stopwatch.Elapsed.TotalSeconds);
        }
        catch (OperationCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return SiteCheckResult.FromTimeOut(url, stopwatch.Elapsed.TotalSeconds);
        }
        catch (HttpRequestException ex)
        {
            return SiteCheckResult.FromHttpRequestFailure(
                url,
                stopwatch.Elapsed.TotalSeconds,
                ex.Message);
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
