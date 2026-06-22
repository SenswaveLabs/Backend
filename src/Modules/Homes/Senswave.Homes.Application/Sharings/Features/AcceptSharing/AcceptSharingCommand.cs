namespace Senswave.Homes.Application.Sharings.Features.AcceptSharing;

public class AcceptSharingCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;

    public string Password { get; set; } = string.Empty;
}