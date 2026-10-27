using PulseWatch.Application.Interfaces;
using PulseWatch.Common.Models;
using System.Net.Sockets;

namespace PulseWatch.Infrastructure.MonitoringInfrastructure;

public class SiteAvailabilityChecker(HttpClient httpClient) : ISiteAvailabilityChecker
{
    public async Task<Result> CheckAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(url, cancellationToken);
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is SocketException socketException &&
                socketException.SocketErrorCode == SocketError.HostNotFound)
            {
                return Result.Failure("The Domain name could not be found.");
            }
        }
        
        return Result.Failure("The HTTP request failed.");
    }
}
