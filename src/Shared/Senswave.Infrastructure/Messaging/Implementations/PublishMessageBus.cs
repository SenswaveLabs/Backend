using MassTransit;
using Senswave.Abstractions.Messaging.Interfaces;

namespace Senswave.Infrastructure.Messaging.Implementations;

public sealed class PublishMessageBus(IPublishEndpoint publishEndpoint, ILogger<PublishMessageBus> logger) : IPublishMessageBus
{
    public async Task Publish<T>(T data, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            logger.LogInformation("Event Type: {@RequestName}, Message reported to message bus.", typeof(T).Name);

            await publishEndpoint.Publish(data, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while publishing message");
        }
    }
}