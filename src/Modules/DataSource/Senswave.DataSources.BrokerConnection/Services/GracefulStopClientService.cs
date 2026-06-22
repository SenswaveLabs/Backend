using Microsoft.Extensions.Logging;
using Senswave.DataSources.BrokerConnection.Interfaces;

namespace Senswave.DataSources.BrokerConnection.Services;

internal sealed class GracefulStopClientService(
    IClientService clientService,
    ILogger<GracefulStopClientService> logger) : IGracefulStopClientService
{
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        await clientService.DisposeAsync();

        logger.LogInformation("All broker clients have been stopped gracefully.");
    }
}
