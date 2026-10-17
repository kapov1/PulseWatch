using System.Diagnostics;

public class MonitoringService(HttpClient httpClient)
{
    private readonly int _timeOut = 5000;

    public async Task<CheckResult> SendRequestAsync(string url)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(_timeOut);
        var token = cts.Token;

        Stopwatch stopwatch = new();

        stopwatch.Start();

        using var response = await httpClient.GetAsync(url, token);

        stopwatch.Stop();

        string message = "";
        if (cts.IsCancellationRequested) message = "Request timed out";

        return new(url, stopwatch.Elapsed.TotalSeconds, response.StatusCode, message);
    } 
}