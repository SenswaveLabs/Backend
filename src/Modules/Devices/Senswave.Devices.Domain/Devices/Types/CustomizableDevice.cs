using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Models;
using Senswave.Devices.Domain.Devices.TileTypes;
using Senswave.Devices.Domain.Operations.Factory;
using System.Text.Json;

namespace Senswave.Devices.Domain.Devices.Types;

public class CustomizableDevice(
    Device device,
    IDeviceTile tile,
    OperationFactory operationFactory,
    ILogger<IDevice> logger) : IDevice
{
    private static readonly Error FailedToCreatePresenceOperation =
        Error.Failure("FailedToCreatePresenceOperation", "Failed to create operation for device presence.");

    public Guid Id => device.Id;

    public IDeviceTile Tile => tile;

    public Device AsEntity() => device;

    public async Task<Result<DisplayDeviceModel>> ToDisplay()
    {
        var model = new DisplayDeviceModel
        {
            Id = device.Id,
            RoomId = device.RoomReferenceId,
            Name = device.Name,
            Icon = device.Icon,
            Tile = new() { Type = device.Tile.Type },
            CreatedAtUtc = device.CreatedAtUtc,
            UpdatedAtUtc = device.UpdatedAtUtc
        };

        try
        {
            if (device.Presence is null || device.Presence.Type == DevicePresenceType.Default)
            {
                model.Presence = new DisplayPresenceModel { Type = DevicePresenceType.Default };
            }
            else if (device.Presence.Type == DevicePresenceType.BooleanOperation)
            {
                var operationResult = operationFactory.Create(device.Presence.Operation!);

                if (operationResult.IsFailure)
                {
                    logger.LogError("[Device: {deviceId}] Failed to create operation for device presence.", device.Id);
                    return Result<DisplayDeviceModel>.Failure(FailedToCreatePresenceOperation);
                }

                var lastValueResult = await operationResult.Data.GetCurrentValue();

                model.Presence = new DisplayPresenceModel
                {
                    Type = DevicePresenceType.BooleanOperation,
                    Value = lastValueResult.IsSuccess && lastValueResult.Data.Value.GetValueKind() == JsonValueKind.True,
                    LastSeenAtUtc = lastValueResult.IsSuccess ? lastValueResult.Data.ProcessedAtUtc : null
                };
            }

            var tileDisplayResult = tile.ToDisplay();
            if (tileDisplayResult.IsSuccess)
                model.Tile = tileDisplayResult.Data;
            else
                model.Tile.Type = DeviceTileType.Invalid;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "[Device: {deviceId}] Error assembling device display.", device.Id);
            model.Tile.Type = DeviceTileType.Invalid;
        }

        return Result<DisplayDeviceModel>.Success(model);
    }
}
