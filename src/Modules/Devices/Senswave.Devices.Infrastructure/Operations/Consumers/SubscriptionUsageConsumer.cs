using Senswave.Devices.Domain.Operations.Repositories;
using Senswave.Integration.Devices.SubscriptionUsage;

namespace Senswave.Devices.Infrastructure.Operations.Consumers;

public class SubscriptionUsageConsumer(
    IOperationCommandRepository commandRepository,
    ILogger<SubscriptionUsageConsumer> logger) : IConsumer<SubscriptionUsageRequest>
{
    public async Task Consume(ConsumeContext<SubscriptionUsageRequest> context)
    {
        var operations = await commandRepository.GetOperationsByReference(
            context.Message.SubscriptionId, context.CancellationToken);

        logger.LogInformation(
            "[Subscription: {subscriptionId}] Found {count} operations referencing this subscription.",
            context.Message.SubscriptionId, operations.Count);

        await context.RespondAsync(new SubscriptionUsageResponse
        {
            OperationsCount = operations.Count
        });
    }
}
