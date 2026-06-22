using Senswave.Users.Domain;

namespace Senswave.Users.Infrastructure.Options;

public class UserServiceOptions
{
    public const string SectionName = $"{UsersOptions.SectionName}:UserService";

    public bool ForceConsentsInRquests { get; set; } = true;

    public int ConsentCacheDurationInSeconds { get; set; } = 600;
}
