using Senswave.Devices.Domain.ShareDevices.Repositories;
using Senswave.Integration.Devices.RemoveDeviceSharings;
using Senswave.Integration.Shared;

namespace Senswave.Devices.Infrastructure.ShareDevices.Consumers;

public class RemoveDeviceSharingConsumer(
    IDeviceSharingCommandRepository repository,
    ILogger<RemoveDeviceSharingConsumer> logger) : IConsumer<RemoveDeviceSharingsRequest>
{
    public async Task Consume(ConsumeContext<RemoveDeviceSharingsRequest> context)
    {
        var result = await repository.DeleteDevicesSharings(context.Message.UserId, context.Message.HomeReferenceId, context.CancellationToken);

        var response = BaseInternalResponse.Success();

        if (result.IsFailure)
        {
            logger.LogError(
                "[UserId: {UserId}] Error while removing device sharings.", context.Message.UserId);
            response = BaseInternalResponse.Failure();
        }
        else
        {
            logger.LogInformation(
                "[UserId: {UserId}] Device sharings removed successfully.",
                context.Message.UserId);
        }


        await context.RespondAsync<RemoveDeviceSharingsResponse>(response);
    }
}
