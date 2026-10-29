using PulseWatch.Application.Interfaces;
using PulseWatch.Common.Models;
using PulseWatch.Domain;

namespace PulseWatch.Application.Services;

public class SiteMonitoringService(
    IMonitoredSiteRepository siteRepository, IHttpSiteChecker siteChecker)
{
    public async Task<ISiteCheckResult> CheckByUrlAsync(
        string url,
        CancellationToken cancellationToken)
    {
        return await siteChecker.CheckSiteAsync(url, cancellationToken);
    }

    public async Task<Result<ISiteCheckResult>> CheckByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var getSiteResult = await siteRepository.GetByIdAsync(id, cancellationToken);

        if (getSiteResult.IsFailure)
        {
            return Result<ISiteCheckResult>.Failure(getSiteResult.ErrorMessage);
        }

        var site = getSiteResult.Value;
        var siteCheckResult = await siteChecker.CheckSiteAsync(site.Url, cancellationToken);

        return Result<ISiteCheckResult>.Success(siteCheckResult);
    }

    public async Task<IEnumerable<ISiteCheckResult>> CheckAllAsync(
        CancellationToken cancellationToken)
    {
        var sites = await siteRepository.GetAllAsync(cancellationToken);

        var urls = sites.Select(site => site.Url);

        return await siteChecker.CheckManyAsync(urls, cancellationToken);
    }

    public async Task<IEnumerable<ISiteCheckResult>> CheckManyAsync(
        IEnumerable<MonitoredSite> sites,
        CancellationToken cancellationToken)
    {
        var urls = sites.Select(site => site.Url);

        return await siteChecker.CheckManyAsync(urls, cancellationToken);
    }
}
