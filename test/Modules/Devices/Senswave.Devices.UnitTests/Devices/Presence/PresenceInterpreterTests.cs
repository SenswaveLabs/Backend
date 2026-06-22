using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.Devices.Application.Devices.Services;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Factory;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.TileTypes;
using Senswave.Devices.Domain.Devices.Types;
using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Operations.ValueObjects;

namespace Senswave.Devices.UnitTests.Devices.Presence;

[Trait("Collection", "UnitTests")]
public class PresenceInterpreterTests
{
    private readonly DeviceService _interpreter;

    public PresenceInterpreterTests()
    {
        var operationFactory = new OperationFactory(
            new Mock<ILogger<IOperation>>().Object,
            new Mock<ILogger<OperationFactory>>().Object);

        var deviceTileFactory = new DeviceTileFactory(
            operationFactory,
            new Mock<ILogger<DeviceTileFactory>>().Object,
            new Mock<ILogger<IDeviceTile>>().Object);
        var deviceFactory = new DeviceFactory(
            deviceTileFactory,
            operationFactory,
            new Mock<IDeviceQueryRepository>().Object,
            new Mock<ILogger<DeviceFactory>>().Object,
            new Mock<ILogger<IDevice>>().Object);
        _interpreter = new DeviceService(
            operationFactory,
            deviceFactory,
            new Mock<IPublishMessageBus>().Object,
            new Mock<IDeviceQueryRepository>().Object,
            new Mock<ILogger<DeviceService>>().Object);
    }

    private static Device BuildDevice(DevicePresence? presence) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Device",
        Icon = "icon.png",
        Tile = new DeviceTile { Type = DeviceTileType.Default },
        Presence = presence,
        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow
    };

    [Fact]
    public async Task Presence_Null_ReturnsDefaultPresence()
    {
        var result = await _interpreter.Interpret(BuildDevice(presence: null));

        Assert.True(result.IsSuccess);
        Assert.Equal(DevicePresenceType.Default, result.Data.Presence.Type);
        Assert.Null(result.Data.Presence.Value);
        Assert.Null(result.Data.Presence.LastSeenAtUtc);
    }

    [Fact]
    public async Task Presence_DefaultType_ReturnsDefaultPresence()
    {
        var presence = new DevicePresence { Type = DevicePresenceType.Default };

        var result = await _interpreter.Interpret(BuildDevice(presence));

        Assert.True(result.IsSuccess);
        Assert.Equal(DevicePresenceType.Default, result.Data.Presence.Type);
        Assert.Null(result.Data.Presence.Value);
        Assert.Null(result.Data.Presence.LastSeenAtUtc);
    }

    [Fact]
    public async Task Presence_BooleanOperation_TrueValue_ReturnsTruePresence()
    {
        var processedAt = DateTime.UtcNow.AddMinutes(-2);
        var presence = new DevicePresence
        {
            Type = DevicePresenceType.BooleanOperation,
            Operation = new Operation
            {
                Type = OperationType.Boolean,
                Values =
                [
                    new() { Value = true, ProcessedAtUtc = processedAt },
                    new() { Value = false, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-10) }
                ]
            }
        };

        var result = await _interpreter.Interpret(BuildDevice(presence));

        Assert.True(result.IsSuccess);
        Assert.Equal(DevicePresenceType.BooleanOperation, result.Data.Presence.Type);
        Assert.True(result.Data.Presence.Value);
        Assert.Equal(processedAt, result.Data.Presence.LastSeenAtUtc);
    }

    [Fact]
    public async Task Presence_BooleanOperation_FalseValue_ReturnsFalsePresence()
    {
        var processedAt = DateTime.UtcNow.AddMinutes(-1);
        var presence = new DevicePresence
        {
            Type = DevicePresenceType.BooleanOperation,
            Operation = new Operation
            {
                Type = OperationType.Boolean,
                Values =
                [
                    new() { Value = false, ProcessedAtUtc = processedAt },
                    new() { Value = true, ProcessedAtUtc = DateTime.UtcNow.AddMinutes(-10) }
                ]
            }
        };

        var result = await _interpreter.Interpret(BuildDevice(presence));

        Assert.True(result.IsSuccess);
        Assert.Equal(DevicePresenceType.BooleanOperation, result.Data.Presence.Type);
        Assert.False(result.Data.Presence.Value);
        Assert.Equal(processedAt, result.Data.Presence.LastSeenAtUtc);
    }

    [Fact]
    public async Task Presence_BooleanOperation_NoValues_ReturnsFalseWithNullTimestamp()
    {
        var presence = new DevicePresence
        {
            Type = DevicePresenceType.BooleanOperation,
            Operation = new Operation { Type = OperationType.Boolean }
        };

        var result = await _interpreter.Interpret(BuildDevice(presence));

        Assert.True(result.IsSuccess);
        Assert.Equal(DevicePresenceType.BooleanOperation, result.Data.Presence.Type);
        Assert.False(result.Data.Presence.Value);
        Assert.Null(result.Data.Presence.LastSeenAtUtc);
    }

    [Fact]
    public async Task Presence_BooleanOperation_InvalidOperationType_ReturnsFailure()
    {
        var presence = new DevicePresence
        {
            Type = DevicePresenceType.BooleanOperation,
            Operation = new Operation { Type = OperationType.Invalid }
        };

        var result = await _interpreter.Interpret(BuildDevice(presence));

        Assert.True(result.IsFailure);
    }
}
