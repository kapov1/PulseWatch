public sealed class UrlResponse(UrlModel urlModel)
{
    public string Url { get; } = urlModel.Url;
    public string Name { get; } = urlModel.Name;
    public int Id { get; } = urlModel.Id;
}