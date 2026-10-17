public class UrlStorage
{
    private readonly List<UrlModel> _urls = new();

    public IReadOnlyList<UrlModel> Urls => _urls;

    public void AddUrl(UrlModel url) => _urls.Add(url);
    public void RemoveUrl(UrlModel url) => _urls.Remove(url);
}