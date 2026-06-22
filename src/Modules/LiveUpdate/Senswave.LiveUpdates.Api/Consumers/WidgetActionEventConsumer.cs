using Senswave.Integration.DataTransfer.Devices;
using Senswave.LiveUpdates.Api.Services.DevicesUpdates;

namespace Senswave.LiveUpdates.Api.Consumers;

public class WidgetActionEventConsumer(
    IDevicesUpdateService service,
    ILogger<WidgetActionEventConsumer> logger) : IConsumer<WidgetActionEvent>
{
    public async Task Consume(ConsumeContext<WidgetActionEvent> context)
    {
        try
        {
            logger.LogInformation("[Device: {deviceId}] Device update for widgets.", context.Message.DeviceId);

            await service.UpdateWidgets(context.Message.DeviceId, context.Message.WidgetIds);

            logger.LogInformation("[Device: {deviceId}] Widgets update sent successfully", context.Message.DeviceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Device: {deviceId}] Failed to send device update.", context.Message.DeviceId);
        }
    }
}
