using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Application.Sharings.Features.CreateSharing;

public class CreateSharingCommand : ICommand<CreateSharingDto>
{
    public Guid OwnerId { get; set; } = Guid.Empty;

    public string FriendEmail { get; set; } = string.Empty;

    public Guid HomeId { get; set; } = Guid.Empty;

    public HomeSharingType SharingType { get; set; }
}