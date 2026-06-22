namespace Senswave.DataSources.Domain.Brokers.Clients.Entities;

public interface IClient : IDisposable
{
    Guid BrokerId { get; }

    Guid SessionId { get; }

    bool IsConnected { get; }

    bool AllowReconnecting { get; }

    bool Remove { get; }

    DateTime DisconnectedAtUtc { get; }

    Task<Result> Start(string username, string password, CancellationToken cancellation = default);
    Task<Result> Stop(CancellationToken cancellation = default);
    Task<Result> Restart(CancellationToken cancellation = default);

    Task<Result> Unsubscribe(string topic, CancellationToken cancellation = default);
    Task<Result> Subscribe(string topic, CancellationToken cancellation = default);
    Task<Result> Publish(string topic, string payload, CancellationToken cancellation = default);
}
