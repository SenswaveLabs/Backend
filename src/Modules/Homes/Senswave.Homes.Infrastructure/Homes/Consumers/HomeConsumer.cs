using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.Home;
using Senswave.Integration.Shared;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class HomeConsumer(
    IHomeQueryRepository repository,
    ILogger<HomeConsumer> logger) : IConsumer<HomeRequest>
{
    public async Task Consume(ConsumeContext<HomeRequest> context)
    {
        var home = await repository.GetHomeWithSharedUsers(context.Message.HomeId, context.CancellationToken);

        if (home is null)
        {
            logger.LogWarning("[Home: {homeId}] Home not found.", context.Message.HomeId);
            await context.RespondAsync<HomeResponse>(BaseInternalResponse.Failure());
            return;
        }

        var response = new HomeResponse
        {
            StatusCode = InternalRequestStatus.Success,
            DataSourceId = home.DataSourceReference?.DataSourceId,
            OwnerId = home.OwnerId,
            AllowedUsers = home.HomeSharing.Select(x => x.UserId).ToList()
        };

        logger.LogInformation("[Home: {homeId}] Home found with {allowedUsersCount} allowed users.",
            context.Message.HomeId, response.AllowedUsers.Count);
        await context.RespondAsync(response);
    }
}
