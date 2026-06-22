using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Clients.Enums;

namespace Senswave.DataSources.Domain.Brokers.Clients.Proxy;

public interface IClientProxy
{
    /// <summary>
    /// By Working I mean that mqtt client was created.
    /// </summary>
    /// <param name="brokerId"></param>
    /// <returns></returns>
    Task<Result> ClientExists(Guid brokerId, CancellationToken cancellationToken);

    Task<Result<ClientState>> ClientState(Guid brokerId, CancellationToken cancellationToken);

    Task<Result> Restart(Guid brokerId, CancellationToken cancellationToken);
    Task<Result<Guid>> Stop(Guid brokerId, CancellationToken cancellationToken);
    Task<Result> Start(Broker broker, Guid sessionId, string username, string password, CancellationToken cancellationToken);
    Task<Result> PublishMessage(Guid brokerId, string payload, string topic, CancellationToken cancellationToken);
}
