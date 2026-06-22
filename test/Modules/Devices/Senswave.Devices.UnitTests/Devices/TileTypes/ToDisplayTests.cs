using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.UnitTests.Devices.TileTypes;

[Trait("Collection", "UnitTests")]
public class ToDisplayTests : BaseTileTest
{
    [Fact]
    public async Task InterpretationFailReturnsInvalidWidget()
    {
        // Arrange
        var device = new Device
        {
            Id = Guid.NewGuid(),
            RoomReferenceId = Guid.NewGuid(),
            Name = "UnsupportedDevice",
            Icon = "unsupported-icon.png",
            Tile = new DeviceTile { Type = (DeviceTileType)999 },
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(DeviceTileType.Invalid, result.Data.Tile.Type);
    }
}
