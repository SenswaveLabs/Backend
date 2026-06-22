using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.ShareDevices.Enums;

namespace Senswave.Devices.Domain.ShareDevices.Entites;

public class DeviceSharing : AuditableEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid DeviceId { get; set; }

    public Device? Device { get; set; }

    [Required]
    public DeviceSharingType SharingType { get; set; }
}