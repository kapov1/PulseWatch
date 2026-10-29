namespace PulseWatch.Application.Interfaces;

public interface ISiteCheckResult
{
    bool? IsReachable { get; }
    bool? IsAvailable { get; }
    int? StatusCode { get; }
    double ResponseTime { get; }
    DateTimeOffset CheckedAt { get; }
    string? FailureReason { get; }
}
