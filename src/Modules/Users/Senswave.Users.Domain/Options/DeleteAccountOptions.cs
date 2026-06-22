namespace Senswave.Users.Domain.Options;

public class DeleteAccountOptions
{
    public const string SectionName = $"{UsersOptions.SectionName}:DeleteAccount";

    public int DelaySeconds { get; set; } = 10;

    public bool WorkerEnabled { get; set; } = true;
}
