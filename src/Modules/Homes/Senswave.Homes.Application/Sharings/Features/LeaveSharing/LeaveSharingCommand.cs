namespace Senswave.Homes.Application.Sharings.Features.LeaveSharing;

public class LeaveSharingCommand : ICommand
{
    public Guid UserId { get; init; } = Guid.Empty;

    public Guid HomeId { get; init; } = Guid.Empty;
}
