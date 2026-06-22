using Senswave.Integration.Shared;
using Senswave.Integration.User;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Infrastructure.Consumers;

public class UserByEmailConsumer(
    IQueryUserRepository queryUserRepository,
    ILogger<UserByEmailConsumer> logger) : IConsumer<UserByEmailRequest>
{
    public async Task Consume(ConsumeContext<UserByEmailRequest> context)
    {
        var user = await queryUserRepository.GetUserByEmail(context.Message.Email, context.CancellationToken);

        if (user is null)
        {
            logger.LogWarning("User not found by email.");
            await context.RespondAsync<UserByEmailResponse>(BaseInternalResponse.Failure());
            return;
        }

        var success = new UserByEmailResponse
        {
            UserId = user.Id,
            StatusCode = InternalRequestStatus.Success
        };

        logger.LogInformation("[ UserId: {UserId}] User found by email.",
            user.Id);

        await context.RespondAsync(success);
    }
}