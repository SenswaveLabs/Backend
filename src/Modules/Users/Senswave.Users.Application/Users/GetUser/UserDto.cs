using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Application.Users.GetUser;

public class UserDto
{
    public required User User { get; set; }

    public bool HasActiveConsent { get; set; } = false;
}
