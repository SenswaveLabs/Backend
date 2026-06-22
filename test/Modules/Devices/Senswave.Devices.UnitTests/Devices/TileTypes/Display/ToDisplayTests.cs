using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Display;

[Trait("Collection", "UnitTests")]
public class ToDisplayTests : BaseDisplayTileTests
{
    [Fact]
    public async Task InterpretedWithNumericValue()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var device = NumberDisplayDevice(
            unit: "°C",
            values:
            [
                new() { Value = 21.5, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) },
                new() { Value = 20.0, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-5) }
            ]);
        device.Id = deviceId;
        device.RoomReferenceId = roomId;
        device.Name = "DisplayDevice";
        device.Icon = "display-icon.png";

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(deviceId, result.Data.Id);
        Assert.Equal(DeviceTileType.Display, result.Data.Tile.Type);
        Assert.Equal("°C", result.Data.Tile.Configuration!["unit"]!.GetValue<string>());
        Assert.NotNull(result.Data.Tile.Value);
        Assert.Equal(21.5, result.Data.Tile.Value.GetValue<double>());
    }

    [Fact]
    public async Task InterpretedWithNullValue()
    {
        // Arrange
        var device = NumberDisplayDevice(unit: "km/h");

        // Act
        var result = await deviceService.Interpret(device);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(DeviceTileType.Display, result.Data.Tile.Type);
        Assert.Equal("km/h", result.Data.Tile.Configuration!["unit"]!.GetValue<string>());
        Assert.Null(result.Data.Tile.Value);
    }

    [Fact]
    public void ReturnsLatestValueByTimestamp()
    {
        var device = NumberDisplayDevice(values:
        [
            new() { Value = 20.0, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-5) },
            new() { Value = 21.5, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) }
        ]);

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var result = tileResult.Data.ToDisplay();
        Assert.True(result.IsSuccess);
        Assert.Equal(21.5, result.Data.Value!.GetValue<double>());
    }
}
