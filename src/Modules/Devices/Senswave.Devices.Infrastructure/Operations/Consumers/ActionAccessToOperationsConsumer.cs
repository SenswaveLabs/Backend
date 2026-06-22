using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Devices.Domain.ShareDevices.Enums;
using Senswave.Integration.Devices.ActionAccessToOperations;
using Senswave.Integration.Shared;

namespace Senswave.Devices.Infrastructure.Operations.Consumers;

public class ActionAccessToOperationsConsumer(
    IOperationQueryRepository queryRepository,
    ILogger<ActionAccessToOperationsConsumer> logger) : IConsumer<ActionAccessToOperationsRequest>
{
    /// <summary>
    /// Consumer assumes that user has at least display access to home (Action for devices). 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<ActionAccessToOperationsRequest> context)
    {
        var devices = await queryRepository.GetDevicesWithSharingsByOperations(context.Message.OperationIds, context.CancellationToken);

        foreach (var dervice in devices)
        {
            var sharingExists = dervice.DeviceSharings.FirstOrDefault(x => x.UserId == context.Message.UserId);

            if (sharingExists is not null)
            {
                if (sharingExists.SharingType == DeviceSharingType.Display)
                {
                    await context.RespondAsync<ActionAccessToOperationsResponse>(BaseInternalResponse.Failure());
                    return;
                }
            }
        }

        logger.LogInformation(
            "[UserId: {UserId}] Action access to operations request processed successfully.",
            context.Message.UserId);
        await context.RespondAsync<ActionAccessToOperationsResponse>(BaseInternalResponse.Success());
    }
}