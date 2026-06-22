using Senswave.Users.Domain.ValueObjects;

namespace Senswave.TestInfrastructure.TestSetup.Models.Users;

public class TestUser
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public RoleTypes Role { get; set; } = RoleTypes.User;
}
