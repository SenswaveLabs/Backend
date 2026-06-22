using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.TileTypes;
using Senswave.Devices.Domain.Devices.TileTypes.Default;
using Senswave.Devices.Domain.Devices.TileTypes.Display;
using Senswave.Devices.Domain.Devices.TileTypes.Switch;
using Senswave.Devices.Domain.Operations.Factory;

namespace Senswave.Devices.Domain.Devices.Factory;

public class DeviceTileFactory(
    OperationFactory operationFactory,
    ILogger<DeviceTileFactory> logger,
    ILogger<IDeviceTile> tileLogger)
{
    private static readonly Error FailedToCreateTile =
        Error.Failure("FailedToCreateTile", "Failed to create device tile.");

    private static readonly Error UnsupportedTileType =
        Error.Failure("UnsupportedDeviceTileType", "Device tile type is not supported.");

    public Result<IDeviceTile> Create(Device device)
    {
        try
        {
            return device.Tile.Type switch
            {
                DeviceTileType.Default => Result<IDeviceTile>.Success(new DefaultDeviceTile(device.Tile)),
                DeviceTileType.Switch => CreateSwitch(device.Tile),
                DeviceTileType.Display => CreateDisplay(device.Tile),
                _ => Result<IDeviceTile>.Failure(UnsupportedTileType),
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Device: {deviceId}] [TileType: {tileType}] Failed to create tile.",
                device.Id, device.Tile.Type);
            return Result<IDeviceTile>.Failure(FailedToCreateTile);
        }
    }

    private Result<IDeviceTile> CreateSwitch(DeviceTile tile)
    {
        if (tile.SwitchOperation is null)
        {
            logger.LogError("[Tile: {tileId}] Switch operation is null.", tile.Id);
            return Result<IDeviceTile>.Failure(Error.Failure("MissingSwitchOperation", "Switch tile requires an operation."));
        }

        var operationResult = operationFactory.Create(tile.SwitchOperation);

        if (operationResult.IsFailure)
        {
            logger.LogError("[Tile: {tileId}] Failed to create operation for switch tile.", tile.Id);
            return Result<IDeviceTile>.Failure(operationResult.Errors);
        }

        return Result<IDeviceTile>.Success(tile.ToSwitchDeviceTile(operationResult.Data, tileLogger));
    }

    private Result<IDeviceTile> CreateDisplay(DeviceTile tile)
    {
        if (tile.DisplayableOperation is null)
        {
            logger.LogError("[Tile: {tileId}] Display operation is null.", tile.Id);
            return Result<IDeviceTile>.Failure(Error.Failure("MissingDisplayOperation", "Display tile requires an operation."));
        }

        var operationResult = operationFactory.Create(tile.DisplayableOperation);

        if (operationResult.IsFailure)
        {
            logger.LogError("[Tile: {tileId}] Failed to create operation for display tile.", tile.Id);
            return Result<IDeviceTile>.Failure(operationResult.Errors);
        }

        return Result<IDeviceTile>.Success(tile.ToDisplayDeviceTile(operationResult.Data, tileLogger));
    }
}
