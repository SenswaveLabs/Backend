using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Devices.Domain.Devices.Types;
using Senswave.Devices.Domain.Operations.Factory;

namespace Senswave.Devices.Domain.Devices.Factory;

public class DeviceFactory(
    DeviceTileFactory deviceTileFactory,
    OperationFactory operationFactory,
    IDeviceQueryRepository queryRepository,
    ILogger<DeviceFactory> logger,
    ILogger<IDevice> deviceLogger)
{
    private static readonly Error FailedToCreateDevice =
        Error.Failure("FailedToCreateDevice", "Failed to create device.");

    private static readonly Error DeviceNotFound =
        Error.Failure("DeviceNotFound", "Device was not found.");

    public async Task<Result<IDevice>> Create(Guid deviceId, CancellationToken cancellationToken)
    {
        var device = await queryRepository.GetDevice(deviceId, cancellationToken);

        if (device is null)
        {
            logger.LogWarning("[Device: {deviceId}] Device not found.", deviceId);
            return Result<IDevice>.Failure(DeviceNotFound);
        }

        return Create(device);
    }

    public Result<IDevice> Create(Device device)
    {
        try
        {
            var tileResult = deviceTileFactory.Create(device);

            if (tileResult.IsFailure)
            {
                logger.LogError("[Device: {deviceId}] [TileType: {tileType}] Failed to create tile.",
                    device.Id, device.Tile.Type);
                return Result<IDevice>.Failure(tileResult.Errors);
            }

            return Result<IDevice>.Success(
                new CustomizableDevice(device, tileResult.Data, operationFactory, deviceLogger));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Device: {deviceId}] Failed to create device.", device.Id);
            return Result<IDevice>.Failure(FailedToCreateDevice);
        }
    }
}
