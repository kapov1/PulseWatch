public class UrlService(UrlStorage urlStorage)
{
    private bool Contains(UrlModel url) => urlStorage.Urls.Any(url_ => url_.Url == url.Url);

    public Result AddUrl(UrlModel url)
    {
        if (Contains(url))
        {
            return Result.Failure("This URL already exist");
        }

        urlStorage.AddUrl(url);

        return Result.Success();
    }

    public Result RemoveUrl(UrlModel url)
    {
        if (!Contains(url)) return Result.Failure("This URL doesn't exist");

        urlStorage.RemoveUrl(url);

        return Result.Success();
    }

    public IEnumerable<UrlModel> FindByName(string name)
    {
        IEnumerable<UrlModel> urls = urlStorage.Urls.Where(url => url.Name == name);

        return urls;
    }
}