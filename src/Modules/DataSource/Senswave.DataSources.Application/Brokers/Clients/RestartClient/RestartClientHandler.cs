using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;

namespace Senswave.DataSources.Application.Brokers.Clients.RestartClient;

public sealed class RestartClientHandler(
    IBrokerCommandRepository brokerRepository,
    IClientProxy clientProxy,
    ILogger<RestartClientHandler> logger) : ICommandHandler<RestartClientCommand>
{
    public async Task<Result> Handle(RestartClientCommand request, CancellationToken cancellationToken)
    {
        var access = await brokerRepository.GetBroker(request.BrokerId, request.UserId, cancellationToken);

        if (access is null)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result.Failure(RestartClientErrors.BrokerNotFound);
        }

        var response = await clientProxy.Restart(request.BrokerId, cancellationToken);

        if (response.IsFailure)
        {
            logger.LogError("[User: {userId}] [Broker: {brokerId}] Failed to restart client.",
                request.UserId, request.BrokerId);
            return Result.Failure(RestartClientErrors.FailedToRestartClient, response.Errors);
        }

        logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Client restarted successfully.", request.UserId, request.BrokerId);
        return Result.Success();
    }
}
