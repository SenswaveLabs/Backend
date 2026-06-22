namespace Senswave.Users.Application.Users.CreateConsents;

public class CreateConsentsCommand : ICommand
{
    public Guid UserId { get; set; }
}
