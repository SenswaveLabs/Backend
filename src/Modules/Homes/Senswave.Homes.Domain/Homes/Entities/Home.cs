using Senswave.Homes.Domain.Homes.ValueObjects;
using Senswave.Homes.Domain.Rooms.Entities;
using Senswave.Homes.Domain.Sharings.Entities;

namespace Senswave.Homes.Domain.Homes.Entities;

public class Home : AuditableEntity
{
    public Guid OwnerId { get; set; }

    public DataSourceReference? DataSourceReference { get; set; }

    [Required]
    [MinLength(AllowedLengths.Names.MinLength)]
    [MaxLength(AllowedLengths.Names.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(AllowedLengths.Icons.MaxLength)]
    public string Icon { get; set; } = string.Empty;

    public Location? Location { get; set; }

    public ICollection<Room> Rooms { get; set; } = [];

    public ICollection<HomeSharing> HomeSharing { get; set; } = [];

    public List<HomeSharingInvitation> HomeSharingInvitations { get; set; } = [];
}