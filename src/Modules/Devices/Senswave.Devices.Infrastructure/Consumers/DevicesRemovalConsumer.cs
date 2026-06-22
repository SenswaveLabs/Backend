using Microsoft.EntityFrameworkCore;
using Senswave.Integration.Devices.Remove;
using Senswave.Integration.Shared;

namespace Senswave.Devices.Infrastructure.Consumers;

public class DevicesRemovalConsumer(
    DevicesContext devicesContext,
    ILogger<DevicesRemovalConsumer> logger) : IConsumer<DevicesRemoveRequest>
{
    public async Task Consume(ConsumeContext<DevicesRemoveRequest> context)
    {
        try
        {
            var devicesAndDependent = await devicesContext.Devices
                .Where(x => x.HomeReference.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            var deviceSharings = await devicesContext.DeviceSharings
                .Where(x => x.UserId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            devicesContext.Devices.RemoveRange(devicesAndDependent);
            devicesContext.DeviceSharings.RemoveRange(deviceSharings);

            var homerReferences = await devicesContext.HomeReferences
                .Where(hr => hr.OwnerId == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            var dataReferences = await devicesContext.DataReferences
                .Where(x => (x.Device!.HomeReference.OwnerId) == context.Message.UserId)
                .ToListAsync(context.CancellationToken);

            devicesContext.DataReferences.RemoveRange(dataReferences);
            devicesContext.HomeReferences.RemoveRange(homerReferences);

            await devicesContext.SaveChangesAsync(context.CancellationToken);
            await context.RespondAsync<DevicesRemoveResponse>(BaseInternalResponse.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[User: {userId}] Error occurred while removing devices.", context.Message.UserId);
            var response = BaseInternalResponse.Failure(Error.ServerError("DevicesRemovalServerError", "Failed to remove devices or linked entites. Please try again or remove devices, dashboards etc. manually.")) as DevicesRemoveResponse;
            await context.RespondAsync(response!);
        }
    }
}
