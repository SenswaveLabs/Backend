using Microsoft.AspNetCore.Identity;
using Senswave.Users.Domain.Enums;

namespace Senswave.Users.Domain.Entity;

public class User : IdentityUser<Guid>
{
    public List<UserConsents> UserConsents { get; set; } = [];

    public Language Language { get; set; } = Language.English;

    public Theme Theme { get; set; } = Theme.Default;
}
