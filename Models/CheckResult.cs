using System.Net;

public sealed record CheckResult
{
    public string Url { get; }
    public double ResponseTime { get; }
    public HttpStatusCode StatusCode { get; }
    public string Message { get; }

    public CheckResult(string url, double responseTime, HttpStatusCode statusCode, string message = "")
    {
        Url = url;
        ResponseTime = responseTime;
        StatusCode = statusCode;
        Message = message;
    }
}