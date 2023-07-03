namespace PismoWebInput.Core.Infrastructure.Domain.Options;

public class MailOptions
{
    public const string SectionKey = "Mail";

    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 25;
    public string DefaultFrom { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}