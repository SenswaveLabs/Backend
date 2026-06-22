namespace Senswave.Users.Application.Auth.Google.Login;

public class GoogleAuthCommand : ICommand
{
    public string Code { get; set; } = string.Empty;
}
