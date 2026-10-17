public class UrlModel
{
    public string Url { get; }
    public string? Name { get; } 

    public UrlModel(string url, string? name = null)
    {
        Url = url;
        Name = name;

        if (name is null) Name = url;
    }
}