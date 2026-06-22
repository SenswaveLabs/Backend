using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.Notifications;

namespace Senswave.DataSources.BrokerConnection.Features.Unsubscribe;

public class UnsubscribeConsumer(
    IClientService clientService,
    ILogger<UnsubscribeConsumer> logger) : IConsumer<UnsubscribeNotifications>
{
    public async Task Consume(ConsumeContext<UnsubscribeNotifications> context)
    {
        logger.LogInformation("[Broker: {brokerId}] Unsubscribing from topic: {topic}.", context.Message.BrokerId, context.Message.Topic);
        await clientService.UnsubscribeTopicFromClient(context.Message.BrokerId, context.Message.Topic, context.CancellationToken);
    }
}
