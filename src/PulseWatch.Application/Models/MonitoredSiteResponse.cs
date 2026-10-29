using PulseWatch.Domain.Models;

namespace PulseWatch.Application.Models;

public class MonitoredSiteResponse(MonitoredSite site)
{
    public int Id { get; } = site.Id;
    public string Name { get; } = site.Name;
    public string Url { get; } = site.Url;
}
