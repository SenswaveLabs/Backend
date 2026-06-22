using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

public interface IBrokerQueryRepository
{
    Task<Broker?> GetBroker(Guid userId, Guid brokerId, CancellationToken cancellationToken);

    Task<Broker?> GetBroker(Guid brokerId, CancellationToken cancellationToken);

    Task<List<Broker>> GetBrokers(Guid userId, int skip, int size, CancellationToken cancellationToken);

    Task<bool> BrokerExists(string url, int port, CancellationToken cancellationToken);

    Task<bool> BrokerNameUsedByUser(string name, Guid userId, CancellationToken cancellationToken);

    Task<bool> ClientNameIsUsedForBroker(string clientName, string url, int port, CancellationToken cancellationToken);

    Task<int> CountBrokersForUser(Guid userId, CancellationToken cancellationToken);

    Task<int> CountGlobalBrokers(CancellationToken cancellationToken);
}
