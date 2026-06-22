using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;

namespace Senswave.DataSources.BrokerConnection.Interfaces;

public interface IClientService : IAsyncDisposable
{
    Task<Result<Guid>> StopClient(Guid brokerId, CancellationToken cancellationToken);
    Task<Result> StartClient(StartClientModel model, CancellationToken cancellation);

    Result<IClient> GetClient(Guid brokerId);
    Result<List<Guid>> GetClientIds();

    Task<Result> PublishToClient(Guid brokerId, string topic, string payload, CancellationToken cancellationToken);
    Task<Result> SubscribeTopicForClient(Guid brokerId, string topic, CancellationToken cancellationToken);
    Task<Result> UnsubscribeTopicFromClient(Guid brokerId, string topic, CancellationToken cancellationToken);
    Task<Result> RestartClient(Guid brokerId, CancellationToken cancellationToken);
}
