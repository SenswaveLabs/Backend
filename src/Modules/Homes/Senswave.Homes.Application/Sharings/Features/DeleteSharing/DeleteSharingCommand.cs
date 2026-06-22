namespace Senswave.Homes.Application.Sharings.Features.DeleteSharing;

public class DeleteSharingCommand : ICommand<Guid>
{
    public Guid HomeSharingId { get; set; } = Guid.Empty;

    public Guid UserId { get; set; } = Guid.Empty;

}