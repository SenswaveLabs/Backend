namespace Senswave.Abstractions.Messaging.Interfaces;

public interface IPublishMessageBus
{
    Task Publish<T>(T data, CancellationToken cancellationToken = default) where T : class;
}
