using System.ComponentModel.DataAnnotations;

public class CreateUrlRequest
{
    [Required]
    [StringLength(500, ErrorMessage = "URL is very long")]
    [UrlAddress]
    public string Url { get; set; } = "";

    [StringLength(50)]
    public string? Name { get; set; }
}