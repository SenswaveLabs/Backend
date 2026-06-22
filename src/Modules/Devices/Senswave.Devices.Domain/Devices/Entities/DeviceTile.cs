using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Operations.Entities;

namespace Senswave.Devices.Domain.Devices.Entities;

public class DeviceTile : AuditableEntity
{
    /* Base Fields */
    public DeviceTileType Type { get; set; } = DeviceTileType.Default;

    /* Required References */
    public Guid DeviceId { get; set; }
    public Device? Device { get; set; }

    /* Optional References */
    public Guid? SwitchOperationId { get; set; } = null;
    public Operation? SwitchOperation { get; set; } = null;

    public Guid? DisplayableOperationId { get; set; } = null;
    public Operation? DisplayableOperation { get; set; } = null;

    public JsonObject Configuration { get; set; } = [];
}
