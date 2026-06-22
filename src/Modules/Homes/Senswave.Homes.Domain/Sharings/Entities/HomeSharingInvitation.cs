using Senswave.Homes.Domain.Homes.Entities;
using Senswave.Homes.Domain.Sharings.Enums;

namespace Senswave.Homes.Domain.Sharings.Entities;

public class HomeSharingInvitation : AuditableEntity
{
    [Required]
    public Guid HomeId { get; set; }

    public Home? Home { get; set; }

    [Required]
    public Guid FriendId { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateTime ExpirationTimeUtc { get; set; }

    public HomeSharingType Type { get; set; }
}