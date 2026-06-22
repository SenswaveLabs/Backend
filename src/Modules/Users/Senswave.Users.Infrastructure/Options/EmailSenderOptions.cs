using Senswave.Users.Domain;

namespace Senswave.Users.Infrastructure.Options;

public class EmailSenderOptions
{
    public const string SectionName = $"{UsersOptions.SectionName}:Sender";

    public string ConfirmationUrl { get; set; } = string.Empty;

    public string ResetUrl { get; set; } = string.Empty;
}
