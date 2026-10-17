public class UrlService(UrlStorage urlStorage)
{
    private static int _idCounter = 1;

    private bool Contains(UrlModel url) => urlStorage.Urls.Any(urlModel => urlModel.Id == url.Id);

    public Result<UrlModel> FindById(int id)
    {
        var urlModel = urlStorage.Urls.FirstOrDefault(_url => _url.Id == id);

        return urlModel is null ?
            Result<UrlModel>.Failure("The URL with this ID doesn't exist") :
            Result<UrlModel>.Success(urlModel);
    }

    public Result<UrlModel> AddUrl(string url, string? name = null)
    {
        UrlModel urlModel = new(_idCounter++, url, name);

        if (Contains(urlModel))
        {
            return Result<UrlModel>.Failure("This URL with this ID already exist");
        }

        urlStorage.AddUrl(urlModel);

        return Result<UrlModel>.Success(urlModel);
    }

    public Result RemoveUrl(int id)
    {
        var result = FindById(id);

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

    public Result<UrlModel> Update(string url, string? name, int id)
    {
        var result = FindById(id);
        if (result.IsFailure) return Result<UrlModel>.Failure(result.ErrorMessage);

        var urlModel = result.Value!;
        urlModel.Update(url, name);

        return Result<UrlModel>.Success(urlModel);
    }
}