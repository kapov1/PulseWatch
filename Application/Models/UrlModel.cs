public class UrlModel
{
    public int Id { get; }
    public string Url { get; private set; }
    public string Name { get; private set; } 

    public UrlModel(int id, string url, string? name = null)
    {
        Url = url;

        if (name is null) Name = url;
        else Name = name;

        Id = id;
    }

    public void Update(string url, string? name)
    {
        Url = url;

        if (name is null) Name = url;
        else Name = name;
    }
}