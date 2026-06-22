using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.Integration.DataSource.BrokerAccess;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.Infrastructure.Brokers.Brokers.Features;

public class BrokerAccessConsumer(
    IBrokerCommandRepository repository,
    ILogger<BrokerAccessConsumer> logger) : IConsumer<BrokerAccessRequest>
{
    private static Error BrokerNotFound => Error.Failure("BrokerNotFound", "Failed to find broker by subscription id.");

    public async Task Consume(ConsumeContext<BrokerAccessRequest> context)
    {
        try
        {
            var broker = await repository.GetBroker(context.Message.BrokerId, context.Message.UserId, context.CancellationToken);

            if (broker is null)
            {
                logger.LogWarning("[Broker: {brokerId}] Failed to find broker for user: {userId}.", context.Message.BrokerId, context.Message.UserId);
                await context.RespondAsync<BrokerAccessResponse>(BaseInternalResponse.Failure(BrokerNotFound));
                return;
            }

            logger.LogInformation("[Broker: {brokerId}] Broker access granted for user: {userId}.", context.Message.BrokerId, context.Message.UserId);
            await context.RespondAsync<BrokerAccessResponse>(BaseInternalResponse.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Broker: {brokerId}] Failed to find broker", context.Message.BrokerId);
            await context.RespondAsync<BrokerAccessResponse>(BaseInternalResponse.Failure(BrokerNotFound));
        }
    }
}
