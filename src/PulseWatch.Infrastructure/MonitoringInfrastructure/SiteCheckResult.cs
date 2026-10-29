using PulseWatch.Application.Interfaces;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteCheckResult : ISiteCheckResult
{
    #region Properties
    public string Url { get; }

    public bool? IsReachable { get; }

    public bool? IsAvailable { get; }

    public int? StatusCode { get; }

    public double ResponseTime { get; }

    public DateTimeOffset CheckedAt { get; }

    public string? FailureReason { get; }
    #endregion

    private SiteCheckResult(string url, HttpResponseMessage httpResponse, double responseTime)
    {
        Url = url;
        IsReachable = true;
        IsAvailable = httpResponse.IsSuccessStatusCode;
        StatusCode = (int)httpResponse.StatusCode;
        ResponseTime = responseTime;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = (bool)IsAvailable ? null : httpResponse.ReasonPhrase;
    }

    private SiteCheckResult(string url, double responseTime, string failureReason)
    {
        Url = url;
        IsReachable = failureReason == "TimeOut" ? null : false;
        IsAvailable = failureReason == "TimeOut" ? null : false;
        StatusCode = null;
        ResponseTime = responseTime;
        CheckedAt = DateTimeOffset.UtcNow;
        FailureReason = failureReason;
    }

    public static SiteCheckResult FromHttpResponse(
        string url,
        HttpResponseMessage httpResponse,
        double responseTime) => new(url, httpResponse, responseTime);

    public static SiteCheckResult FromHostNotFound(
        string url,
        double responseTime) => new(url, responseTime, "HostNotFound");


    public static SiteCheckResult FromHttpRequestFailure(
        string url,
        double responseTime,
        string errorMessage) => new(url, responseTime, errorMessage);

    public static SiteCheckResult FromTimeOut(
        string url,
        double responseTimeMs) => new(url, responseTimeMs, "TimeOut");
}