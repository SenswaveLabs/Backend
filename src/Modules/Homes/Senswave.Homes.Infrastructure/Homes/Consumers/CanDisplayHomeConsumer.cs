using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.CanDisplayHome;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class CanDisplayHomeConsumer(
    IHomeQueryRepository queryRepository,
    ILogger<CanManageHomeConsumer> logger) : IConsumer<CanDisplayHomeRequest>
{
    public async Task Consume(ConsumeContext<CanDisplayHomeRequest> context)
    {
        try
        {
            var isOwner = await queryRepository.IsHomeOwner(context.Message.UserId, context.Message.HomeId, context.CancellationToken);

            if (isOwner)
            {
                logger.LogInformation("[Home: {homeId}][User: {userId}] User is the owner of the home, can display home.",
                    context.Message.HomeId,
                    context.Message.UserId);
                await context.RespondAsync<CanDisplayHomeResponse>(BaseInternalResponse.Success());
                return;
            }

            var canDisplay = await queryRepository.CanDisplayHome(context.Message.UserId, context.Message.HomeId, context.CancellationToken);

            if (canDisplay)
            {
                logger.LogInformation("[Home: {homeId}][User: {userId}] User can display the home.",
                    context.Message.HomeId,
                    context.Message.UserId);
                await context.RespondAsync<CanDisplayHomeResponse>(BaseInternalResponse.Success());
                return;
            }

            logger.LogWarning("[Home: {homeId}][User: {userId}] User cannot display the home.",
                context.Message.HomeId,
                context.Message.UserId);
            await context.RespondAsync<CanDisplayHomeResponse>(BaseInternalResponse.Failure());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Home: {homeId}][Device: {deviceId}] Error while checking if user can manage home.",
                context.Message.HomeId,
                context.Message.UserId);

            await context.RespondAsync<CanDisplayHomeResponse>(BaseInternalResponse.Failure());
        }
    }
}
