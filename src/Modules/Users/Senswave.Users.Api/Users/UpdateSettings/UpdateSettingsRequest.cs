namespace Senswave.Users.Api.Users.UpdateSettings;

public record UpdateSettingsRequest
{
    public string Language { get; init; } = string.Empty;
    public string Theme { get; init; } = string.Empty;
}
