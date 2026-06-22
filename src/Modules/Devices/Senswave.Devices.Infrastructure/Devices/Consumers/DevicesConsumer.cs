using Senswave.Devices.Domain.Devices.Repositories;
using Senswave.Integration.Devices.Devices;

namespace Senswave.Devices.Infrastructure.Devices.Consumers;

public class DevicesConsumer(
    IDeviceQueryRepository repository,
    ILogger<DevicesConsumer> logger) : IConsumer<DevicesRequest>
{
    public async Task Consume(ConsumeContext<DevicesRequest> context)
    {
        try
        {
            var devices = await repository.GetDevicesByHome(context.Message.HomeId, context.CancellationToken);

            var response = new DevicesResponse
            {
                StatusCode = Integration.Shared.InternalRequestStatus.Success,
                Devices = devices
            };

            logger.LogInformation("[Home: {homeId}] Devices request processed successfully.", context.Message.HomeId);
            await context.RespondAsync(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Home: {homeId}] Error while processing devices request for home.", context.Message.HomeId);
            await context.RespondAsync<DevicesResponse>(DevicesResponse.Failure());
        }
    }
}
