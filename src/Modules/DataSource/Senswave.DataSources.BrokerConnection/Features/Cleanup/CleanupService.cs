using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;
using Senswave.DataSources.Domain.Brokers.Clients.Options;

namespace Senswave.DataSources.BrokerConnection.Features.Cleanup;

public class CleanupService(IClientService clientService,
    IOptions<ClientCleanupOptions> cleanupOptions,
    ILogger<CleanupService> logger) : ICleanupService
{

    private readonly ClientCleanupOptions _configuration = cleanupOptions.Value
    ?? throw new ArgumentNullException("Missing cleanup configuration");

    #region Errors

    private static readonly Error FailedToGetClients = Error.Failure("FailedToGetClients", "Failed to get clients for cleanup.");
    private static readonly Error OperationTimedOut = Error.Failure("OperationTimedOut", "Operation timed out.");

    #endregion

    public Result RemoveUnusedClients(CancellationToken cancellationToken)
    {
        var clientsResult = clientService.GetClientIds();

        if (clientsResult.IsFailure)
        {
            logger.LogError("[Cleanup] Failed to get clients for cleanup.");
            return Result.Failure(FailedToGetClients);
        }

        foreach (var client in clientsResult.Data)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("[Cleanup] Cleanup operation is stopping due to cancellation token.");
                return Result.Failure([OperationTimedOut]);
            }
            _ = StopConnectionIfApplicative(client);
        }

        return Result.Success();

    }

    #region Privates

    private async Task StopConnectionIfApplicative(Guid clientIdentifier)
    {
        var exists = clientService.GetClient(clientIdentifier);

        if (exists.IsFailure)
            return;

        if (!ShouldBeRemoved(exists.Data))
            return;

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        logger.LogInformation("[Client Cleaner][Client: {ClientId}] Transfering client to cleanup.", clientIdentifier);
        var result = await clientService.StopClient(clientIdentifier, cts.Token);

        if (result.IsFailure)
            logger.LogError("[Client Cleaner][Client: {ClientId}] Failed to transfer client to cleanup.", clientIdentifier);
    }
    private bool ShouldBeRemoved(IClient client) => client.Remove &&
        DateTime.UtcNow - client.DisconnectedAtUtc > TimeSpan.FromMilliseconds(_configuration.ClientCleanupSpanMiliseconds);

    #endregion
}
