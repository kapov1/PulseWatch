namespace PulseWatch.Application.Interfaces;

public interface ISiteCheckResult
{
    bool IsAvailable { get; }
    int? StatusCode { get; }
    string? StatusCodeName { get; } 
    int ResponseTimeMs { get; }
    DateTimeOffset CheckedAt { get; }
    string? FailureReason { get; }
}
