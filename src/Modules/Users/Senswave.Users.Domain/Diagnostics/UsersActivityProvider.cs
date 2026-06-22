using Senswave.Abstractions.Diagnostics;

namespace Senswave.Users.Domain.Diagnostics;

public sealed class UsersActivityProvider : ActivityProvider, IUsersActivityProvider
{
    public const string DefaultListenerName = "Senswave.Users";

    public UsersActivityProvider() : base(DefaultListenerName)
    {
    }
}