namespace Senswave.Users.Infrastructure.Options;

public class EmailServiceOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Password { get; set; } = string.Empty;

    public string User { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;

    public bool UseSsl { get; set; }

    public bool Enabled { get; set; }
}
