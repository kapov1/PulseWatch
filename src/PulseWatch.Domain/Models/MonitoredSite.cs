namespace PulseWatch.Domain.Models;

public class MonitoredSite(string name, string url)
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public string Url { get; set; } = url;
}
