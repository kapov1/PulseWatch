namespace PulseWatch.Application.Interfaces;

public interface ISiteCheckResult
{
    bool? IsReachable { get; }
    bool? IsAvailable { get; }
    int? StatusCode { get; }
    double ResponseTimeMs { get; }
    DateTimeOffset CheckedAt { get; }
    string? FailureReason { get; }
}
