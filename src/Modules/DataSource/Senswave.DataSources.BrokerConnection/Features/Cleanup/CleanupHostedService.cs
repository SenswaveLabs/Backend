using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Options;

namespace Senswave.DataSources.BrokerConnection.Features.Cleanup;

public class CleanupHostedService(
    IGracefulStopClientService stopClientService,
    ICleanupService cleanupService,
    IOptions<ClientCleanupOptions> cleanupOptions,
    ILogger<CleanupHostedService> logger)
    : BackgroundService
{
    private readonly ClientCleanupOptions _configuration = cleanupOptions?.Value
        ?? throw new ArgumentNullException("Missing cleanup configuration");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_configuration.Enabled)
        {
            logger.LogInformation("[Client Cleaner] Service is enabled.");
            return;
        }

        logger.LogInformation("[Client Cleaner] Service is starting.");

        await RemoveOutdatedClients(stoppingToken);

        logger.LogInformation("[Client Cleaner] Service is stopping.");

        await stopClientService.StopAsync(default);

        logger.LogInformation("[Client Cleaner] Service stopped.");
    }

    public async Task RemoveOutdatedClients(CancellationToken stoppingToken)
    {
        try
        {
            var delay = TimeSpan.FromMinutes(_configuration.CleanupSpanMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                cleanupService.RemoveUnusedClients(stoppingToken);
                await Task.Delay(delay, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("[Client Cleaner] Service is stopping due to canncellation token.");
        }
    }
}
