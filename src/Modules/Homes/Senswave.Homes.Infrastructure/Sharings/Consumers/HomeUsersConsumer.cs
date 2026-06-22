using Senswave.Homes.Domain.Sharings.Extensions;
using Senswave.Homes.Domain.Sharings.Repositories;
using Senswave.Integration.Homes.HomeUsers;

namespace Senswave.Homes.Infrastructure.Sharings.Consumers;

public sealed class HomeUsersConsumer(
    IHomeSharingQueryRepository repository,
    ILogger<HomeUsersConsumer> logger) : IConsumer<HomeUsersRequest>
{
    public async Task Consume(ConsumeContext<HomeUsersRequest> context)
    {
        var users = await repository.GetSharingUsers(context.Message.HomeId, context.CancellationToken);

        var response = new HomeUsersResponse
        {
            Users = users.Select(x => new HomeUsersResponse.HomeUser
            {
                UserId = x.UserId,
                HomeSharingType = x.SharingType.FromHomeSharingType()
            }).ToList()
        };

        logger.LogInformation("[Home: {homeId}] Found {usersCount} users sharing this home.",
            context.Message.HomeId, response.Users.Count);
        await context.RespondAsync(response);
    }
}
