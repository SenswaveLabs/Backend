using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.IsHomeOwner;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

public class IsHomeOwnerConsumer(
    IHomeQueryRepository queryRepository,
    ILogger<IsHomeOwnerConsumer> logger)
    : IConsumer<IsHomeOwnerRequest>
{
    public async Task Consume(ConsumeContext<IsHomeOwnerRequest> context)
    {
        var homeOwnership = await queryRepository.IsHomeOwner(context.Message.UserId, context.Message.HomeId,
            context.CancellationToken);

        var response = new IsHomeOwnerResponse { StatusCode = InternalRequestStatus.Failure };

        if (homeOwnership)
        {
            logger.LogInformation("[Home: {homeId}][User: {userId}] User is the owner of the home.",
                context.Message.HomeId, context.Message.UserId);
            response.StatusCode = InternalRequestStatus.Success;
        }
        else
        {
            logger.LogWarning("[Home: {homeId}][User: {userId}] User is not the owner of the home.",
                context.Message.HomeId, context.Message.UserId);
        }

        await context.RespondAsync(response);
    }
}