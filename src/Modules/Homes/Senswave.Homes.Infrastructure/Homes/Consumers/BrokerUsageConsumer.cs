using Senswave.Homes.Domain.Homes.Repositories;
using Senswave.Integration.Homes.BrokerUsedInHome;

namespace Senswave.Homes.Infrastructure.Homes.Consumers;

internal sealed class BrokerUsageConsumer(
    IHomeCommandRepository repository,
    ILogger<BrokerUsageConsumer> logger) : IConsumer<BrokerUsageRequest>
{
    public async Task Consume(ConsumeContext<BrokerUsageRequest> context)
    {
        var homesUsingBroker = await repository.CountHomeByDataSourceId(context.Message.BrokerId, context.CancellationToken);

        logger.LogInformation("[Broker: {brokerId}] Found {homesCount} homes using this broker.", context.Message.BrokerId, homesUsingBroker);
        await context.RespondAsync(new BrokerUsageResponse { UsedInHomes = homesUsingBroker });
    }
}
