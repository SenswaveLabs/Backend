using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Application.Sharings.Features.GetSharings;

public class SharingModel
{
    public Guid SharingId { get; set; }

    public string FriendEmail { get; set; } = string.Empty;

    public HomeSharingType SharingType { get; set; }
}