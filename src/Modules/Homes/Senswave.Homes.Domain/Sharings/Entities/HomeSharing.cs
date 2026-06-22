using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Domain.Sharings.Entities;

public class HomeSharing : AuditableEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid HomeId { get; set; }

    [Required]
    public Home Home { get; set; }

    [Required]
    public HomeSharingType SharingType { get; set; }
}