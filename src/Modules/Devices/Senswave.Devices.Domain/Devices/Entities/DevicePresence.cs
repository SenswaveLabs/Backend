

using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Operations.Entities;

namespace Senswave.Devices.Domain.Devices.Entities;

public class DevicePresence : AuditableEntity
{
    public DevicePresenceType Type { get; set; } = DevicePresenceType.Default;

    /* Required References */
    public Guid DeviceId { get; set; }
    public Device? Device { get; set; }

    /* Optional References */
    public Guid? OperationId { get; set; }
    public Operation? Operation { get; set; }
}