namespace Senswave.Users.Api.Users.GetUser;

public class GetUserResponse
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public bool HasActiveConsent { get; set; } = false;
}
