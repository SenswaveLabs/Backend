using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.BrokerConnection.RateLimiters;
using Senswave.DataSources.Domain.Brokers.Clients.Options;
using Senswave.DataSources.Domain.Diagnostics;

namespace Senswave.DataSources.BrokerConnection.Clients;

public class MqttV5Client(
        Guid brokerId,
        Guid sessionId,
        MqttClientOptionsBuilder optionsBuilder,
        ClientOptions options,
        IBrokerRateLimiter brokerRateLimiter,
        IList<string> subscribtions,
        IMqttClient client,
        IMessageBus messageBus,
        IDataSourcesActivityProvider activityProvider,
        ILogger<BaseClient> logger)
    : BaseClient(brokerId, sessionId, optionsBuilder, options, brokerRateLimiter, subscribtions, client, messageBus, activityProvider, logger)
{
    #region Protected

    protected override MqttClientSubscribeOptions BuildSubscribtion(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithNoLocal(true)
            .Build();

        return new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(topicFilter)
            .Build();
    }

    #endregion
}