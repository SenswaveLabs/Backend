using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.CanManageHome;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class CanManageHomeConsumer(
    IHomeQueryRepository queryRepository,
    ILogger<CanManageHomeConsumer> logger) : IConsumer<CanManageHomeRequest>
{
    public async Task Consume(ConsumeContext<CanManageHomeRequest> context)
    {
        try
        {
            var isOwner = await queryRepository.IsHomeOwner(context.Message.UserId, context.Message.HomeId, context.CancellationToken);

            if (isOwner)
            {
                logger.LogInformation("[Home: {homeId}][User: {userId}] User is the owner of the home, can manage home.",
                    context.Message.HomeId,
                    context.Message.UserId);
                await context.RespondAsync<CanManageHomeResponse>(BaseInternalResponse.Success());
                return;
            }

            var canManage = await queryRepository.CanManageHome(context.Message.UserId, context.Message.HomeId, context.CancellationToken);

            if (canManage)
            {
                logger.LogInformation("[Home: {homeId}][User: {userId}] User can manage the home.",
                    context.Message.HomeId,
                    context.Message.UserId);
                await context.RespondAsync<CanManageHomeResponse>(BaseInternalResponse.Success());
                return;
            }

            logger.LogWarning("[Home: {homeId}][User: {userId}] User cannot manage the home.",
                context.Message.HomeId,
                context.Message.UserId);
            await context.RespondAsync<CanManageHomeResponse>(BaseInternalResponse.Failure());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Home: {homeId}][Device: {deviceId}] Error while checking if user can manage home.",
                context.Message.HomeId,
                context.Message.UserId);

            await context.RespondAsync<CanManageHomeResponse>(BaseInternalResponse.Failure());
        }
    }
}
