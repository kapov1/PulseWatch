using System.Diagnostics;
using System.Net;

public class MonitoringService(HttpClient httpClient)
{
    private readonly int _timeOut = 5000;

    public async Task<CheckResult> SendRequestAsync(string url)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(_timeOut);
        var token = cts.Token;

        Stopwatch stopwatch = new();   

        try
        {
            stopwatch.Start();

            using var response = await httpClient.GetAsync(url, token);
            
            stopwatch.Stop();

            return new(url, stopwatch.Elapsed.TotalSeconds, response.StatusCode);
        }
        catch (OperationCanceledException)
        {
            return new(url, stopwatch.Elapsed.TotalSeconds, HttpStatusCode.GatewayTimeout, "Request time out");
        }
        catch (HttpRequestException)
        {
            return new(url, stopwatch.Elapsed.TotalSeconds, HttpStatusCode.BadGateway, "Network error");
        }
    }

    public async Task<IEnumerable<CheckResult>> SendRequestsAsync(UrlStorage urlStorage)
    {
        if (!urlStorage.Urls.Any()) return [];

        List<Task<CheckResult>> results = new();

        foreach (var urlModel in urlStorage.Urls)
        {
            results.Add(SendRequestAsync(urlModel.Url));
        }

        var checkResults = await Task.WhenAll(results);

        return checkResults;
    }
}