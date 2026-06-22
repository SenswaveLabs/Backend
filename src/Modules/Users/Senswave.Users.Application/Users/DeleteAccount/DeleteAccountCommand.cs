namespace Senswave.Users.Application.Users.DeleteAccount;

public class DeleteAccountCommand : ICommand
{
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
