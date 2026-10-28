using PulseWatch.Application.Interfaces;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteCheckResult : ISiteCheckResult
{
    public string Url { get; }

    public bool IsReachable { get; }

    public bool IsAvailable { get; }

    public int? StatusCode { get; }

    public double ResponseTimeMs { get; }

    public DateTimeOffset CheckedAt { get; }

    public string? FailureReason { get; }

    private SiteCheckResult(string url, HttpResponseMessage httpResponse, double responseTimeMs)
    {
        Url = url;
        IsReachable = true;
        IsAvailable = httpResponse.IsSuccessStatusCode;
        StatusCode = (int)httpResponse.StatusCode;
        ResponseTimeMs = responseTimeMs;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = IsAvailable ? null : httpResponse.ReasonPhrase;
    }

    private SiteCheckResult(string url, double responseTimeMs, string failureReason)
    {
        Url = url;
        IsReachable = false;
        IsAvailable = false;
        StatusCode = null;
        ResponseTimeMs = responseTimeMs;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = failureReason;
    }

    public static SiteCheckResult FromHttpResponse(string url, HttpResponseMessage httpResponse, double responseTimeMs) =>
        new(url, httpResponse, responseTimeMs);
    
    public static SiteCheckResult FromHostNotFound(string url, double responseTimeMs) =>
        new(url, responseTimeMs, "HostNotFound");

    public static SiteCheckResult FromHttpRequestFailure(string url, double responseTimeMs) =>
        new(url, responseTimeMs, "HttpRequestFailed");

    public static SiteCheckResult FromTimeOut(string url, double responseTimeMs) => 
        new(url, responseTimeMs, "TimedOut");
}    