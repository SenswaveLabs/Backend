using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.Notifications;

namespace Senswave.DataSources.BrokerConnection.Features.Subscribe;

public class SubscribeConsumer(
    IClientService clientService,
    ILogger<SubscribeConsumer> logger) : IConsumer<SubscribeNotification>
{
    public async Task Consume(ConsumeContext<SubscribeNotification> context)
    {
        logger.LogInformation("[Broker: {brokerId}] Subscribing to topic: {topic}.", context.Message.BrokerId, context.Message.Topic);
        await clientService.SubscribeTopicForClient(context.Message.BrokerId, context.Message.Topic, context.CancellationToken);
    }
}
