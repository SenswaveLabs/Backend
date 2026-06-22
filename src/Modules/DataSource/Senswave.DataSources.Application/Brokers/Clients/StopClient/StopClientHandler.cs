using Senswave.DataSources.Domain.Brokers.Brokers.Repositories;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.ClientExists;
using Senswave.Integration.DataSource.BrokerConnection.Stop;

namespace Senswave.DataSources.Application.Brokers.Clients.StopClient;

internal class StopClientHandler(
    IClientProxy clientProxy,
    ISessionCommandRepository sessionRepository,
    IBrokerCommandRepository brokerRepository,
    ILogger<StopClientHandler> logger)
    : ICommandHandler<StopClientCommand>
{
    public async Task<Result> Handle(StopClientCommand request, CancellationToken cancellationToken)
    {
        var broker = await brokerRepository.GetBroker(request.BrokerId, request.UserId, cancellationToken);

        if (broker is null)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.BrokerNotFound);
        }

        var clientExists = await clientProxy.ClientExists(request.BrokerId, cancellationToken);

        if (clientExists.IsFailure)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId}] Client does not exist or is not running.", request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.ClientIsNotRunning);
        }

        var response = await clientProxy.Stop(request.BrokerId, cancellationToken);

        if (response.IsFailure)
        {
            logger.LogError("[User: {userId}] [Broker: {brokerId}] Failed to stop client.",
                request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.FailedToStopClient);
        }

        await sessionRepository.FinishSession(response.Data, cancellationToken);
        logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Client stopped successfully.", request.UserId, request.BrokerId);
        return Result.Success();
    }

    #region Messages

    public static ClientExistsRequest ClientExists(Guid brokerId) => new()
    {
        BrokerId = brokerId,
    };

    public static StopClientRequest StopClient(Guid brokerId) => new()
    {
        BrokerId = brokerId
    };

    #endregion
}

