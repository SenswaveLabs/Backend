namespace Senswave.Abstractions.Messaging.Interfaces;

public interface IMessageBus
{
    Task Publish<T>(T data, CancellationToken cancellationToken = default) where T : class;
}
