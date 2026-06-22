using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Operations.ValueObjects;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Switch;

[Trait("Collection", "UnitTests")]
public class PreprocessValueTests : BaseSwitchTileTests
{
    private void SetupDevice(Guid deviceId, IList<OperationValue>? values = null)
    {
        var device = BooleanSwitchDevice(deviceId, values: values);
        deviceQueryRepository
            .Setup(r => r.GetDevice(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);
    }

    [Fact]
    public async Task ReturnsTrueWhenHasNoOperationValue()
    {
        var deviceId = Guid.NewGuid();
        SetupDevice(deviceId, values: []);

        var result = await deviceService.PreprocessValueForTileMessage(deviceId, JsonValue.Create("")!, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data.Value.GetValue<bool>());
    }

    [Fact]
    public async Task ReturnsFalseWhenLastValueTrue()
    {
        var deviceId = Guid.NewGuid();
        SetupDevice(deviceId, values:
        [
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow },
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) }
        ]);

        var result = await deviceService.PreprocessValueForTileMessage(deviceId, JsonValue.Create("")!, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Data.Value.GetValue<bool>());
    }

    [Fact]
    public async Task ReturnsTrueWhenLastValueFalse()
    {
        var deviceId = Guid.NewGuid();
        SetupDevice(deviceId, values:
        [
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow },
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) }
        ]);

        var result = await deviceService.PreprocessValueForTileMessage(deviceId, JsonValue.Create("")!, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data.Value.GetValue<bool>());
    }

    [Fact]
    public async Task UsesProvidedTrueValue()
    {
        var deviceId = Guid.NewGuid();
        SetupDevice(deviceId, values:
        [
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow },
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) }
        ]);

        var result = await deviceService.PreprocessValueForTileMessage(deviceId, JsonValue.Create(true)!, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data.Value.GetValue<bool>());
    }

    [Fact]
    public async Task UsesProvidedFalseValue()
    {
        var deviceId = Guid.NewGuid();
        SetupDevice(deviceId, values:
        [
            new() { Value = false, ProcessedAtUtc = DateTime.UtcNow },
            new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-1) }
        ]);

        var result = await deviceService.PreprocessValueForTileMessage(deviceId, JsonValue.Create(false)!, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Data.Value.GetValue<bool>());
    }

    [Fact]
    public async Task FailsWhenDeviceNotFound()
    {
        var deviceId = Guid.NewGuid();
        deviceQueryRepository
            .Setup(r => r.GetDevice(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        var result = await deviceService.PreprocessValueForTileMessage(deviceId, JsonValue.Create(1)!, CancellationToken.None);

        Assert.True(result.IsFailure);
    }
}
