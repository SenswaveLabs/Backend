using Senswave.Integration.DataTransfer.Devices;
using Senswave.LiveUpdates.Api.Services.DevicesUpdates;

namespace Senswave.LiveUpdates.Api.Consumers;

public class DevicePresenceEventConsumer(
    IDevicesUpdateService service,
    ILogger<DevicePresenceEventConsumer> logger) : IConsumer<DevicePresenceEvent>
{
    public async Task Consume(ConsumeContext<DevicePresenceEvent> context)
    {
        try
        {
            logger.LogInformation("[Device: {deviceId}] Device presence update.", context.Message.DeviceId);

            await service.UpdateDevicePresence(context.Message.DeviceId);

            logger.LogInformation("[Device: {deviceId}] Device presence update sent successfully.", context.Message.DeviceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Device: {deviceId}] Failed to send device presence update.", context.Message.DeviceId);
        }
    }
}
