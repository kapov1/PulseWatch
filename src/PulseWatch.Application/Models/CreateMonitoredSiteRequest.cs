using System.ComponentModel.DataAnnotations;

namespace PulseWatch.Application.Models;

public class CreateMonitoredSiteRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;
}