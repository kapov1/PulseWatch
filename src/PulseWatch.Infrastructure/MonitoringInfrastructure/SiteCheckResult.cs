using Npgsql.Replication;
using PulseWatch.Application.Interfaces;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteCheckResult : ISiteCheckResult
{
    public bool IsAvailable { get; }

    public int? StatusCode { get; }

    public string? StatusCodeName { get; }

    public int ResponseTimeMs { get; }

    public DateTimeOffset CheckedAt { get; }

    public string? FailureReason { get; }

    private SiteCheckResult(HttpResponseMessage httpResponse, int responseTimeMs)
    {
        IsAvailable = httpResponse.IsSuccessStatusCode;
        StatusCode = (int)httpResponse.StatusCode;
        StatusCodeName = httpResponse.StatusCode.ToString();
        ResponseTimeMs = responseTimeMs;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = IsAvailable ? null : httpResponse.ReasonPhrase;
    }

    private SiteCheckResult(int responseTimeMs)
    {
        IsAvailable = false;
        StatusCode = null;
        StatusCodeName = null;
        ResponseTimeMs = responseTimeMs;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = "HostNotFound";
    }

    public static SiteCheckResult FromHttpResponse(HttpResponseMessage httpResponse, int responseTimeMs) =>
        new(httpResponse, responseTimeMs);
    
    public static SiteCheckResult FromHostNotFound(int responseTimeMs) =>
        new(responseTimeMs);
}    