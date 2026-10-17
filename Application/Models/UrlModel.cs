public class UrlModel
{
    public string Url { get; private set; }
    public string Name { get; private set; } 

    public UrlModel(string url, string? name = null)
    {
        Url = url;

        if (name is null) Name = url;
        else Name = name;
    }

    public void Update(string url, string? name)
    {
        Url = url;

        if (name is null) Name = url;
        else Name = name;
    }
}