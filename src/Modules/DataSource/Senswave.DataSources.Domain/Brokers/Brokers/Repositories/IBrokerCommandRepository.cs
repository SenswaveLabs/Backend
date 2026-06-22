using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Repositories;

public interface IBrokerCommandRepository
{
    Task<Result> CreateBroker(Broker broker, CancellationToken cancellationToken);
    Task<Result> DeleteBroker(Broker broker, CancellationToken cancellationToken);
    Task<Result> UpdateBroker(Broker broker, CancellationToken cancellationToken);

    Task<Broker?> GetBroker(Guid brokerId, Guid userId, CancellationToken cancellationToken);
}
