using Senswave.Integration.DataTransfer.Devices;
using Senswave.LiveUpdates.Api.Services.DevicesUpdates;

namespace Senswave.LiveUpdates.Api.Consumers;

public class DeviceTileActionEventConsumer(
    IDevicesUpdateService service,
    ILogger<DeviceTileActionEventConsumer> logger) : IConsumer<DeviceTileActionEvent>
{
    public async Task Consume(ConsumeContext<DeviceTileActionEvent> context)
    {
        try
        {
            logger.LogInformation("[Device: {deviceID}] Device update", context.Message.DeviceId);

            await service.UpdateDeviceTile(context.Message.DeviceId);

            logger.LogInformation("[Device: {deviceID}] Device tile update sent successfully", context.Message.DeviceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Device: {deviceID}] Failed to send device update", context.Message.DeviceId);
        }
    }
}
