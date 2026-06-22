using Senswave.Integration.User;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Infrastructure.Consumers;

public class UsersEmailsConsumer(
    IQueryUserRepository queryUserRepository,
    ILogger<UsersEmailsConsumer> logger) : IConsumer<EmailRequest>
{
    public async Task Consume(ConsumeContext<EmailRequest> context)
    {
        var ids = context.Message.UserIds;

        var users = await queryUserRepository.GetUsersByIds(ids, context.CancellationToken);
        var idToEmail = new Dictionary<Guid, string>();

        foreach (var user in users)
        {
            idToEmail.Add(user.Id, user.Email ?? string.Empty);
        }

        logger.LogInformation("Responding with emails for {count} users.",
            idToEmail.Count);

        await context.RespondAsync<EmailResponse>(new EmailResponse { UserEmails = idToEmail });

    }
}