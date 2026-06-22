namespace Senswave.Users.Application.Auth.Google.LoginV2;

public class GoogleJwtAuthCommand : ICommand
{
    public string JwtToken { get; set; } = string.Empty;
}
