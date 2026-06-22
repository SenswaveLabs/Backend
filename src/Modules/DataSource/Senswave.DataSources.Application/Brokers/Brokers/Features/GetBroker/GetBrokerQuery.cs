using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetBroker;

public class GetBrokerQuery : IQuery<Broker>
{
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid BrokerId { get; set; } = Guid.Empty;
}
