using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Services;

public interface IBrokerService
{
    public Task<Result> TestConnection(
        string url,
        int port,
        string clientName,
        bool useTls,
        BrokerProtocolVersion brokerProtocolVersion,
        string username,
        string password,
        CancellationToken cancellationToken);
}
