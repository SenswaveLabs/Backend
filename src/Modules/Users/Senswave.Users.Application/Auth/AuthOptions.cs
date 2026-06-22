using Senswave.Users.Application.Auth.Google;
using Senswave.Users.Domain;

namespace Senswave.Users.Application.Auth;

public class AuthOptions
{
    public const string SectionName = $"{UsersOptions.SectionName}:Auth";

    public GoogleAuthOptions Google { get; set; } = new();
}
