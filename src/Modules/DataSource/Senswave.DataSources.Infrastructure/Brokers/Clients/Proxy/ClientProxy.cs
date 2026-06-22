using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;
using Senswave.DataSources.Domain.Brokers.Clients.Enums;
using Senswave.DataSources.Domain.Brokers.Clients.Proxy;
using Senswave.Integration.DataSource.BrokerConnection.ClientExists;
using Senswave.Integration.DataSource.BrokerConnection.ClientState;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataSource.BrokerConnection.Restart;
using Senswave.Integration.DataSource.BrokerConnection.Start;
using Senswave.Integration.DataSource.BrokerConnection.Stop;

namespace Senswave.DataSources.Infrastructure.Brokers.Clients.Proxy;

internal sealed class ClientProxy(
    IRequestClient<ClientExistsRequest> clientExistsRequestClient,
    IRequestClient<StopClientRequest> stopRequestClient,
    IRequestClient<StartClientRequest> startRequestClient,
    IRequestClient<RestartClientRequest> restartRequestClient,
    IRequestClient<ClientStateRequest> clientStateRequestClient,
    IRequestClient<PublishMessageRequest> publishMessageRequestClient,
    ILogger<ClientProxy> logger)
    : IClientProxy
{
    public async Task<Result> PublishMessage(Guid brokerId, string payload, string topic, CancellationToken cancellationToken)
    {
        var request = new PublishMessageRequest
        {
            BrokerId = brokerId,
            Topic = topic,
            Payload = payload
        };

        var response = await publishMessageRequestClient.GetResponse<PublishMessageResponse>(request, cancellationToken);

        if (response.Message.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Message published successfully to topic: {topic}.", brokerId, topic);
            return Result.Success();
        }

        logger.LogError("[Broker: {brokerId}] Failed to publish message to topic: {topic}. Error: {error}", brokerId, topic, response.Message.Error);
        return Result.Failure([]);
    }

    public async Task<Result<ClientState>> ClientState(Guid brokerId, CancellationToken cancellationToken)
    {
        var request = new ClientStateRequest
        {
            BrokerId = brokerId,
        };

        var response = await clientStateRequestClient.GetResponse<ClientStateResponse>(request, cancellationToken);

        if (response.Message.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client state retrieved successfully.", brokerId);
            return Result<ClientState>.Success((ClientState)response.Message.ClientState);
        }

        logger.LogError("[Broker: {brokerId}] Failed to retrieve client state. Error: {error}", brokerId, response.Message.Error);
        return Result<ClientState>.Failure([]);
    }

    public async Task<Result> ClientExists(Guid brokerId, CancellationToken cancellationToken)
    {
        var request = new ClientExistsRequest
        {
            BrokerId = brokerId,
        };

        var response = await clientExistsRequestClient.GetResponse<ClientExistsResponse>(request, cancellationToken);

        if (response.Message.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client exists.", brokerId);
            return Result.Success();
        }

        logger.LogWarning("[Broker: {brokerId}] Client does not exist.", brokerId);
        return Result.Failure([]);
    }

    public async Task<Result> Restart(Guid brokerId, CancellationToken cancellationToken)
    {
        var request = new RestartClientRequest
        {
            BrokerId = brokerId
        };

        var response = await restartRequestClient.GetResponse<RestartClientResponse>(request, cancellationToken);

        if (response.Message.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client restarted successfully.", brokerId);
            return Result.Success();
        }

        logger.LogError("[Broker: {brokerId}] Failed to restart client. Error: {error}", brokerId, response.Message.Error);
        return Result.Failure([response.Message.Error]);
    }

    public async Task<Result> Start(Broker broker, Guid sessionId, string username, string password, CancellationToken cancellationToken)
    {
        var request = new StartClientRequest()
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

        var response = await startRequestClient.GetResponse<StartClientResponse>(request, cancellationToken);

        if (response.Message.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client started successfully with session ID: {sessionId}.", broker.Id, sessionId);
            return Result.Success();
        }

        logger.LogError("[Broker: {brokerId}] Failed to start client. Error: {error}", broker.Id, response.Message.Error);
        return Result.Failure([]);
    }

    public async Task<Result<Guid>> Stop(Guid brokerId, CancellationToken cancellationToken)
    {
        var request = new StopClientRequest()
        {
            BrokerId = brokerId
        };

        var response = await stopRequestClient.GetResponse<StopClientResponse>(request, cancellationToken);

        if (response.Message.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client stopped successfully with session ID: {sessionId}.", brokerId, response.Message.SessionId);
            return Result<Guid>.Success(response.Message.SessionId);
        }

        logger.LogError("[Broker: {brokerId}] Failed to stop client. Error: {error}", brokerId, response.Message.Error);
        return Result<Guid>.Failure([]);
    }
}
