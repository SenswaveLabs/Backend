namespace Senswave.Users.Application.Auth.Google;

public class GoogleAuthOptions
{
    public const string SectionName = $"{AuthOptions.SectionName}:Google";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
