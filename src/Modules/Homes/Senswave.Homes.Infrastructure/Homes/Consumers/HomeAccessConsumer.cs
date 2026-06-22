using Senswave.Homes.Domain.Sharings.Enums;
using Senswave.Homes.Domain.Sharings.Extensions;
using Senswave.Homes.Domain.Sharings.Repositories;
using Senswave.Integration.Homes.HomeAccess;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class HomeAccessConsumer(
    IHomeSharingQueryRepository repository,
    ILogger<HomeAccessConsumer> logger) : IConsumer<HomeAccessRequest>
{
    public async Task Consume(ConsumeContext<HomeAccessRequest> context)
    {
        var accessType = context.Message.SharingType.ToHomeSharingType();

        if (accessType == HomeSharingType.Invalid || accessType == HomeSharingType.Empty)
        {
            logger.LogError("Invalid sharing type: {sharingType}", context.Message.SharingType);
            await context.RespondAsync(Response(false, false));
            return;
        }

        var accessGranted = await repository
            .UserCanReadHome(context.Message.UserId, context.Message.HomeId, accessType, context.CancellationToken);

        var hasBroker = await repository.HomeHasDataSource(context.Message.HomeId, context.CancellationToken);

        if (hasBroker)
        {
            logger.LogInformation("[Home: {homeId}][User: {userId}] User has access to the home with broker.",
                context.Message.HomeId,
                context.Message.UserId);
        }
        else
        {
            logger.LogWarning("[Home: {homeId}][User: {userId}] User does not have access to the home with broker.",
                context.Message.HomeId,
                context.Message.UserId);
        }
        await context.RespondAsync(Response(hasBroker, accessGranted));
    }

    private static HomeAccessResponse Response(bool hasBroker, bool accessGranted) => new()
    {
        HasBroker = hasBroker,
        StatusCode = accessGranted ? InternalRequestStatus.Success : InternalRequestStatus.Failure
    };
}
