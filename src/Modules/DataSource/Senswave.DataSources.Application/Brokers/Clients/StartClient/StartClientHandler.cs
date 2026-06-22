using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Start;

namespace Senswave.DataSources.Application.Brokers.Clients.StartClient;

internal sealed class StartClientHandler(
    IClientProxy clientProxy,
    ISessionQueryRepository queryRepository,
    ISessionCommandRepository sessionRepository,
    ILogger<StartClientHandler> logger) : ICommandHandler<StartClientCommand>
{
    public async Task<Result> Handle(StartClientCommand request, CancellationToken cancellationToken)
    {
        var broker = await queryRepository.GetBroker(request.UserId, request.BrokerId, cancellationToken);

        if (broker is null)
        {
            logger.LogWarning("[User: {userId}] Broker with id = {brokerId} not found.", request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.BrokerNotFound);
        }

        var clientExists = await clientProxy.ClientExists(request.BrokerId, cancellationToken);

        if (clientExists.IsSuccess)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId}] Client is already running.", request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.ClientIsRunning);
        }

        var endOldSessions = await sessionRepository.FinishSessions(request.BrokerId, cancellationToken);

        if (!endOldSessions)
        {
            logger.LogWarning("[User: {userId}] [Broker: {brokerId}] Failed to finish old sessions.", request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.FailedToEndOldSessions);
        }

        var newSession = new Session
        {
            BrokerId = request.BrokerId,

            Finished = false,
            Logs = [],

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var createSessionResult = await sessionRepository.CreateSession(newSession, cancellationToken);

        if (!createSessionResult)
        {
            logger.LogError("[User: {userId}] [Broker: {brokerId}] Failed to create new session.", request.UserId, request.BrokerId);
            return Result.Failure(StartClientErrors.FailedToStartSession);
        }

        var result = await clientProxy.Start(broker, newSession.Id, request.Username, request.Password, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("[User: {userId}] [Broker: {brokerId}] Client started successfully.", request.UserId, request.BrokerId);
            return Result.Success();
        }

        await sessionRepository.FinishSession(newSession, cancellationToken);
        logger.LogError("[User: {userId}] [Broker: {brokerId}] Failed to start client.", request.UserId, request.BrokerId);

        return Result.Failure(StartClientErrors.FailedToStartSession);
    }

    #region Messages

    public static StartClientRequest StartClient(Broker broker, Guid sessionId, string username, string password) => new()
    {
        BrokerId = broker.Id,
        SessionId = sessionId,

        Server = broker.BrokerInfo.Url,
        ClientName = broker.BrokerInfo.ClientName,
        Port = broker.BrokerInfo.Port,
        Protocol = broker.BrokerInfo.ProtocolVersion.FromProtocol(),
        UseTls = broker.BrokerInfo.UseTls,

        Subscribtions = broker.Subscribtions.Select(x => x.Topic).ToList(),

        Username = username,
        Password = password
    };

    #endregion
}
