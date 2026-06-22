using Senswave.Devices.Domain.Devices.Enums;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Default;

[Trait("Collection", "UnitTests")]
public class ToDisplayTests : BaseDefaultTileTests
{
    [Fact]
    public async Task InterpretedCorrectly()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var device = DefaultTileDevice(deviceId, roomId);

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(deviceId, result.Data.Id);
        Assert.Equal(roomId, result.Data.RoomId);
        Assert.Equal("Device1", result.Data.Name);
        Assert.Equal("icon.png", result.Data.Icon);
        Assert.Null(result.Data.Tile.Value);
    }
}
