using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Switch;

[Trait("Collection", "UnitTests")]
public class ToDisplayTests : BaseSwitchTileTests
{
    [Fact]
    public async Task InterpretedWithTrueValue()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var device = BooleanSwitchDevice(deviceId, roomId, values:
        [
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) },
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-5) }
        ]);

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(deviceId, result.Data.Id);
        Assert.Equal(roomId, result.Data.RoomId);
        Assert.Equal("SwitchDevice", result.Data.Name);
        Assert.Equal("switch-icon.png", result.Data.Icon);
        Assert.NotNull(result.Data.Tile);
        Assert.True(result.Data.Tile.Value!.GetValue<bool>());
    }

    [Fact]
    public async Task InterpretedWithFalseValue()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var device = BooleanSwitchDevice(deviceId, roomId, values:
        [
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) },
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-5) }
        ]);

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(deviceId, result.Data.Id);
        Assert.Equal(roomId, result.Data.RoomId);
        Assert.Equal("SwitchDevice", result.Data.Name);
        Assert.Equal("switch-icon.png", result.Data.Icon);
        Assert.NotNull(result.Data.Tile);
        Assert.False(result.Data.Tile.Value!.GetValue<bool>());
    }

    [Fact]
    public async Task InterpretedWithNullValue()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var device = BooleanSwitchDevice(deviceId, roomId, values: []);

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(deviceId, result.Data.Id);
        Assert.Equal(roomId, result.Data.RoomId);
        Assert.Equal("SwitchDevice", result.Data.Name);
        Assert.Equal("switch-icon.png", result.Data.Icon);
        Assert.NotNull(result.Data.Tile);
        Assert.Null(result.Data.Tile.Value);
    }

    [Fact]
    public void ReturnsLatestValueByTimestamp()
    {
        var device = BooleanSwitchDevice(values:
        [
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-5) },
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) }
        ]);

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var result = tileResult.Data.ToDisplay();
        Assert.True(result.IsSuccess);
        Assert.False(result.Data.Value!.GetValue<bool>());
    }
}
