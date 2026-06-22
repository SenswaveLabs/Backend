using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Switch;

public abstract class BaseSwitchTileTests : BaseTileTest
{
    protected Device BooleanSwitchDevice(
        Guid? deviceId = null,
        Guid? roomId = null,
        IList<OperationValue>? values = null)
    {
        var operationId = Guid.NewGuid();
        return new()
        {
            Id = deviceId ?? Guid.NewGuid(),
            RoomReferenceId = roomId ?? Guid.Empty,
            Name = "SwitchDevice",
            Icon = "switch-icon.png",
            Tile = new DeviceTile
            {
                Type = DeviceTileType.Switch,
                SwitchOperationId = operationId,
                SwitchOperation = new Operation
                {
                    Id = operationId,
                    Type = OperationType.Boolean,
                    Values = values?.ToList() ?? []
                }
            },
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }
}
