using Senswave.Users.Domain.Options;

namespace Senswave.Users.Domain;

public class UsersOptions
{
    public const string SectionName = "Modules:Users";

    public DeleteAccountOptions DeleteAccount { get; set; } = new();

    public ConfirmEmailV2Options ConfirmEmailV2 { get; set; } = new();
}
