public class UrlService(UrlStorage urlStorage)
{
    private bool Contains(UrlModel url) => urlStorage.Urls.Any(urlModel => urlModel.Url == url.Url);

    public Result<UrlModel> AddUrl(string url, string? name = null)
    {
        UrlModel urlModel = new(url, name);

        if (Contains(urlModel))
        {
            return Result<UrlModel>.Failure("This URL already exist");
        }

        urlStorage.AddUrl(urlModel);

        return Result<UrlModel>.Success(urlModel);
    }

    public Result RemoveUrl(string url)
    {
        var result = FindUrlModel(url);

        if (result.IsFailure) return Result.Failure(result.ErrorMessage);

        urlStorage.RemoveUrl(result.Value!);

        return Result.Success();
    }

    public IEnumerable<UrlModel> FindByName(string name)
    {
        IEnumerable<UrlModel> urls = urlStorage.Urls
            .Where(url => url.Name == name);

        return urls;
    }

    public Result<UrlModel> FindUrlModel(string url)
    {
        var urlModel = urlStorage.Urls.FirstOrDefault(_url => _url.Url == url);

        return urlModel is null ? 
            Result<UrlModel>.Failure("This URL doesn't exist") : 
            Result<UrlModel>.Success(urlModel);
    }
}