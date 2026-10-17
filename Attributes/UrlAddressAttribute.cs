using System.ComponentModel.DataAnnotations;

public sealed class UrlAddressAttribute : ValidationAttribute
{
    private readonly string[] _allowedProtocols = ["http://", "https://"];

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null or string) return ValidationResult.Success;

        string url = (string)value;

        foreach (var allowedProtocol in _allowedProtocols)
        {
            if (url.StartsWith(allowedProtocol)) return ValidationResult.Success;
        }

        return new("The URL must be start with allowed protocol");
    }
}