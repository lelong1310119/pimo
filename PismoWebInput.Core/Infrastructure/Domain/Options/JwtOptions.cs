namespace PismoWebInput.Core.Infrastructure.Domain.Options;

public class JwtOptions
{
    public const string SectionKey = "Jwt";

    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double DurationInMinutes { get; set; }
}