using Senswave.Homes.Domain.Rooms.Repositories;
using Senswave.Integration.Homes.RoomAccess;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Rooms.Consumers;

public class RoomAccessConsumer(
    IRoomQueryRepository repository,
    ILogger<RoomAccessConsumer> logger) : IConsumer<RoomAccessRequest>
{
    public async Task Consume(ConsumeContext<RoomAccessRequest> context)
    {
        var exists = await repository.RoomExistsInHome(context.Message.RoomId, context.Message.HomeId, context.CancellationToken);

        var response = exists ? BaseInternalResponse.Success() : BaseInternalResponse.Failure();

        if (exists)
        {
            logger.LogInformation("[Room: {roomId}][Home: {homeId}] User has access to the room.",
                context.Message.RoomId,
                context.Message.HomeId);
        }
        else
        {
            logger.LogWarning("[Room: {roomId}][Home: {homeId}] User does not have access to the room.",
                context.Message.RoomId,
                context.Message.HomeId);
        }

        await context.RespondAsync<RoomAccessResponse>(response);
    }
}