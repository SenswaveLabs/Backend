using Senswave.Abstractions.Resulting;

namespace Senswave.DataSources.BrokerConnection.Features.Cleanup;

public interface ICleanupService
{
    Result RemoveUnusedClients(CancellationToken cancellationToken);
}
