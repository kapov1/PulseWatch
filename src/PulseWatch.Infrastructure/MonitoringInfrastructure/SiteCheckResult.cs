using PulseWatch.Application.Interfaces;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteCheckResult : ISiteCheckResult
{
    public bool IsReachable { get; }

    public bool IsAvailable { get; }

    public int? StatusCode { get; }

    public double ResponseTimeMs { get; }

    public DateTimeOffset CheckedAt { get; }

    public string? FailureReason { get; }

    private SiteCheckResult(HttpResponseMessage httpResponse, double responseTimeMs)
    {
        IsReachable = true;
        IsAvailable = httpResponse.IsSuccessStatusCode;
        StatusCode = (int)httpResponse.StatusCode;
        ResponseTimeMs = responseTimeMs;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = IsAvailable ? null : httpResponse.ReasonPhrase;
    }

    private SiteCheckResult(double responseTimeMs, string failureReason)
    {
        IsReachable = false;
        IsAvailable = false;
        StatusCode = null;
        ResponseTimeMs = responseTimeMs;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = failureReason;
    }

    public static SiteCheckResult FromHttpResponse(HttpResponseMessage httpResponse, double responseTimeMs) =>
        new(httpResponse, responseTimeMs);
    
    public static SiteCheckResult FromHostNotFound(double responseTimeMs) =>
        new(responseTimeMs, "HostNotFound");

    public static SiteCheckResult FromHttpRequestFailure(double responseTimeMs) =>
        new(responseTimeMs, "HttpRequestFailed");

}    