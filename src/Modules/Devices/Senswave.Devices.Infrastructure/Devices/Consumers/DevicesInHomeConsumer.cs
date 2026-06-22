using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Integration.Devices.DevicesInHome;

namespace Senswave.Devices.Infrastructure.Devices.Consumers;

internal class DevicesInHomeConsumer(
    IDeviceQueryRepository repository,
    ILogger<DevicesInHomeConsumer> logger) : IConsumer<DevicesInHomeRequest>
{
    public async Task Consume(ConsumeContext<DevicesInHomeRequest> context)
    {
        var devicesCount = await repository
            .CountDevicesForHome(context.Message.HomeId, context.CancellationToken);

        logger.LogInformation(
            "[Home: {HomeId}] Devices count in home: {DevicesCount}",
            context.Message.HomeId, devicesCount);
        await context.RespondAsync(new DevicesInHomeResponse()
        {
            DevicesCount = devicesCount
        });
    }
}
