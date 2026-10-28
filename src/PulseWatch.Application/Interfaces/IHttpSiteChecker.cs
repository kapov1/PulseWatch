namespace PulseWatch.Application.Interfaces;

public interface IHttpSiteChecker
{
    Task<ISiteCheckResult> CheckSiteAsync(string url, CancellationToken cancellationToken);

    Task<IEnumerable<ISiteCheckResult>> CheckManyAsync(IEnumerable<string> urls, CancellationToken cancellationToken);
}
