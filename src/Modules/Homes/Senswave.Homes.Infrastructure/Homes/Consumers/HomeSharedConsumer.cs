using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.HomeShared;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class HomeSharedConsumer(
    IHomeQueryRepository repository,
    ILogger<HomeSharedConsumer> logger) : IConsumer<HomeSharedRequest>
{
    public async Task Consume(ConsumeContext<HomeSharedRequest> context)
    {
        var isHomeShared = await repository.IsHomeShared(context.Message.UserId, context.Message.HomeId, context.CancellationToken);

        var response = BaseInternalResponse.Failure();

        if (isHomeShared)
        {
            logger.LogInformation("[Home: {homeId}] User {userId} has access to the home.",
                context.Message.HomeId, context.Message.UserId);
            response = BaseInternalResponse.Success();
        }
        else
        {
            logger.LogWarning("[Home: {homeId}] User {userId} does not have access to the home.",
                context.Message.HomeId, context.Message.UserId);

        }

        await context.RespondAsync<HomeSharedResponse>(response);
    }
}
