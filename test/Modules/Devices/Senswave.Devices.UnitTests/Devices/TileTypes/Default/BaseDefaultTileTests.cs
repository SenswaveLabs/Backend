using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Default;

public abstract class BaseDefaultTileTests : BaseTileTest
{
    protected Device DefaultTileDevice(Guid? deviceId = null, Guid? roomId = null) => new()
    {
        Id = deviceId ?? Guid.NewGuid(),
        RoomReferenceId = roomId ?? Guid.Empty,
        Name = "Device1",
        Icon = "icon.png",
        Tile = new DeviceTile { Type = DeviceTileType.Default },
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };
}
