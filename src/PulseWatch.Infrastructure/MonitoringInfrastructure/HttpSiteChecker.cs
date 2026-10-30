using Microsoft.Extensions.Logging;
using PulseWatch.Application.Interfaces;
using System.Diagnostics;
using System.Net.Sockets;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class HttpSiteChecker(
    HttpClient httpClient,
    ILogger<HttpSiteChecker> logger) : IHttpSiteChecker
{
    private static readonly SemaphoreSlim semaphore = new(10, 10);

    private async Task<ISiteCheckResult> CheckWithSemaphoreAsync(
        string url,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            return await CheckSiteAsync(
                url, 
                cancellationToken);
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
            checkTasks.Add(CheckWithSemaphoreAsync(
                url, 
                cancellationToken));
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

            using var response = 
                await httpClient.GetAsync(url, cancellationToken);

            var checkResult = SiteCheckResult.FromHttpResponse(
                url, response, stopwatch.Elapsed.TotalSeconds);

            #region Log
                logger.LogDebug("""
                        Received HTTP response from {Host}. 
                        Status: {StatusCode}, 
                        Available: {IsAvailable}, 
                        Reachable: {IsReachable},
                        Response time: {ResponseTimeSeconds} seconds.
                    """, 
                    checkResult.Url,
                    checkResult.StatusCode,
                    checkResult.IsAvailable,
                    checkResult.IsReachable,
                    checkResult.ResponseTime);
            #endregion

            return SiteCheckResult.FromHttpResponse(
                url,
                response,
                stopwatch.Elapsed.TotalSeconds);
        }
        
        catch (HttpRequestException ex) when (
            ex.InnerException is SocketException socketException &&
            socketException.SocketErrorCode == SocketError.HostNotFound)
        {
            var checkResult = SiteCheckResult.FromHostNotFound(
                url, stopwatch.Elapsed.TotalSeconds);

            #region Log
                logger.LogWarning("""
                        Received HTTP response from {Host}. 
                        Status: {StatusCode}, 
                        Available: {IsAvailable}, 
                        Reachable: {IsReachable},
                        Response time: {ResponseTimeSeconds} seconds.
                    """, 
                    checkResult.Url,
                    checkResult.StatusCode,
                    checkResult.IsAvailable,
                    checkResult.IsReachable,
                    checkResult.ResponseTime);
            #endregion

            return checkResult;
        }
        
        catch (OperationCanceledException ex) when (
            ex.InnerException is TimeoutException)
        {
            var checkResult = SiteCheckResult.FromTimeOut(
                url, stopwatch.Elapsed.TotalSeconds);

            #region Log
                logger.LogWarning("""
                        Received HTTP response from {Host}. 
                        Status: {StatusCode}, 
                        Available: {IsAvailable}, 
                        Reachable: {IsReachable},
                        Response time: {ResponseTimeSeconds} seconds.
                    """, 
                    checkResult.Url,
                    checkResult.StatusCode,
                    checkResult.IsAvailable,
                    checkResult.IsReachable,
                    checkResult.ResponseTime);
            #endregion

            return SiteCheckResult.FromTimeOut(
                url, 
                stopwatch.Elapsed.TotalSeconds);
        }
        
        catch (HttpRequestException ex)
        {
            var checkResult = SiteCheckResult.FromHttpRequestFailure(
                url, stopwatch.Elapsed.TotalSeconds, ex.Message);

            #region Log
                logger.LogWarning("""
                        Received HTTP response from {Host}. 
                        Status: {StatusCode}, 
                        Available: {IsAvailable}, 
                        Reachable: {IsReachable},
                        Response time: {ResponseTimeSeconds} seconds.
                    """, 
                    checkResult.Url,
                    checkResult.StatusCode,
                    checkResult.IsAvailable,
                    checkResult.IsReachable,
                    checkResult.ResponseTime);
            #endregion

            return checkResult;
        }
        
        finally
        {
            stopwatch.Stop();
        }
    }
}
