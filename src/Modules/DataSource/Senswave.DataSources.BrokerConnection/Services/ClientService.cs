using MediatR;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Factories;
using Senswave.DataSources.BrokerConnection.Features.Start;
using Senswave.DataSources.BrokerConnection.Features.Terminate;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;
using System.Collections.Concurrent;

namespace Senswave.DataSources.BrokerConnection.Services;

public class ClientService(ILogger<ClientService> logger,
    IMediator mediator,
    ClientFactory clientFactory) : IClientService
{
    #region Errors

    private static Error ConnectionAlreadyExists => Error.Failure("ConnectionAlreadyExists", "A connection to this broker already exists.");

    private static Error FailedToStartConnection => Error.Failure("FailedToStartConnection", "Failed to start the broker connection.");

    private static Error ClientNotFound => Error.NotFound("ClientDoesNotExist", "The MQTT client does not exist.");

    private static Error ServiceIsStopped => Error.NotFound("ServiceIsStopped", "The connection service is stopped.");

    #endregion

    private readonly ConcurrentDictionary<Guid, IClient> _clients = new();

    private readonly AsyncReaderWriterLock _disposeLock = new();

    private bool serviceStopped = false;

    public async Task<Result> StartClient(StartClientModel model, CancellationToken cancellationToken)
    {
        var readerLock = await _disposeLock.ReaderLockAsync(cancellationToken);

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}][SessionId: {sessionId}] Service is stopping, cannot start new client.",
                    model.BrokerId,
                    model.SessionId);

                return Result.Failure(ServiceIsStopped);
            }

            if (_clients.ContainsKey(model.BrokerId))
            {
                logger.LogWarning("[BrokerId: {brokerId}][SessionId: {sessionId}] Broker client already exists.",
                    model.BrokerId,
                    model.SessionId);

                return Result.Failure(ConnectionAlreadyExists);
            }

            var result = clientFactory.Create(model);

            if (result.IsFailure)
            {
                logger.LogError("[BrokerId: {brokerId}][SessionId: {sessionId}] Client factory failed to create client.",
                    model.BrokerId,
                    model.SessionId);
                return result;
            }

            var client = result.Data;

            var startResult = await client.Start(model.Username, model.Password, cancellationToken);

            if (startResult.IsFailure)
            {
                logger.LogError("[BrokerId: {brokerId}][SessionId: {sessionId}] Client failed to start connection.",
                    model.BrokerId,
                    model.SessionId);

                await TerminateClient(client, cancellationToken);
                return startResult;
            }

            var isAdded = _clients.TryAdd(model.BrokerId, client);

            if (isAdded)
            {
                logger.LogInformation("[BrokerId: {brokerId}][SessionId: {sessionId}] Client starting and adding to collection.",
                    model.BrokerId,
                    model.SessionId);

                return Result.Success();
            }

            logger.LogError("[BrokerId: {brokerId}][SessionId: {sessionId}] Connection with was not added to connection list.",
                model.BrokerId,
                model.SessionId);

            await TerminateClient(client, cancellationToken);

            return Result.Failure(FailedToStartConnection);
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public async Task<Result<Guid>> StopClient(Guid brokerId, CancellationToken cancellationToken)
    {
        //Possible problematic situation with Client cleanup
        // - client disconnected
        // - user clicks restart (start)
        // - in mentime client is being removed by clenaup
        // - start will return succes but right after client will be stopped and removed.
        // Some kind of deadlock if first client will be stopped and then right after started again?
        // Trick Is Used that after 60 seconds client is stopped a

        var readerLock = await _disposeLock.ReaderLockAsync(cancellationToken);

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}] Service is stopping.",
                    brokerId);
                return Result<Guid>.Failure(ServiceIsStopped);
            }

            if (!_clients.TryRemove(brokerId, out var client))
            {
                logger.LogWarning("[BrokerId: {brokerId}] Broker client does not exists.",
                    brokerId);

                return Result<Guid>.Failure(ClientNotFound);
            }

            await TerminateClient(client, cancellationToken);

            logger.LogInformation("[BrokerId: {brokerId}] Broker client stopped and removed from collection.",
                brokerId);

            return Result<Guid>.Success(client.SessionId);
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public async Task<Result> RestartClient(Guid brokerId, CancellationToken cancellationToken)
    {
        var readerLock = await _disposeLock.ReaderLockAsync(cancellationToken);

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}] Service is stopping. Cannot restart client",
                    brokerId);

                return Result.Failure(ServiceIsStopped);
            }

            if (!_clients.TryGetValue(brokerId, out var client))
            {
                logger.LogWarning("[BrokerId: {brokerId}] Broker client does not exists.", brokerId);
                return Result.Failure(ClientNotFound);
            }

            var restart = await client.Restart(cancellationToken);

            logger.LogInformation("[BrokerId: {brokerId}] Broker client restarted.", brokerId);

            return restart;
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public async Task<Result> PublishToClient(Guid brokerId, string topic, string payload, CancellationToken cancellationToken)
    {
        var readerLock = await _disposeLock.ReaderLockAsync(cancellationToken);

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}] Service is stopping. Cannot publish message.",
                    brokerId);

                return Result.Failure(ServiceIsStopped);
            }

            if (!_clients.TryGetValue(brokerId, out var client))
            {
                logger.LogWarning("[BrokerId: {brokerId}] Broker client does not exists.",
                    brokerId);

                return Result.Failure(ClientNotFound);
            }

            var result = await client.Publish(topic, payload, cancellationToken);

            logger.LogInformation("[BrokerId: {brokerId}] Broker client published message.",
                brokerId);

            return result;
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public async Task<Result> SubscribeTopicForClient(Guid brokerId, string topic, CancellationToken cancellationToken)
    {
        var readerLock = await _disposeLock.ReaderLockAsync(cancellationToken);

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}] Service is stopping. Cannot subscribe to topic.",
                    brokerId);
                return Result.Failure(ServiceIsStopped);
            }

            if (!_clients.TryGetValue(brokerId, out var client))
            {
                logger.LogWarning("[BrokerId: {brokerId}] Broker client does not exists.",
                    brokerId);

                return Result.Failure(ClientNotFound);
            }

            return await client.Subscribe(topic, cancellationToken);
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public async Task<Result> UnsubscribeTopicFromClient(Guid brokerId, string topic, CancellationToken cancellationToken)
    {
        var readerLock = await _disposeLock.ReaderLockAsync(cancellationToken);

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}] Service is stopping. Cannot unsubscribe from topic.",
                    brokerId);
                return Result.Failure(ServiceIsStopped);
            }

            if (!_clients.TryGetValue(brokerId, out var client))
            {
                logger.LogWarning("[BrokerId: {brokerId}] Broker client does not exists.",
                    brokerId);
                return Result.Failure(ClientNotFound);
            }

            return await client.Unsubscribe(topic, cancellationToken);
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public Result<IClient> GetClient(Guid brokerId)
    {
        var readerLock = _disposeLock.ReaderLock();

        try
        {
            if (serviceStopped)
            {
                logger.LogWarning("[BrokerId: {brokerId}] Service is stopping.",
                    brokerId);

                return Result<IClient>.Failure(ServiceIsStopped);
            }

            if (!_clients.TryGetValue(brokerId, out var client))
            {
                logger.LogWarning("[BrokerId: {brokerId}] Broker client does not exists.",
                    brokerId);

                return Result<IClient>.Failure(ClientNotFound);
            }

            logger.LogInformation("[BrokerId: {brokerId}] Broker client found.",
                brokerId);

            return Result<IClient>.Success(client);
        }
        finally
        {
            readerLock.Dispose();
        }
    }

    public Result<List<Guid>> GetClientIds()
    {
        var clients = _clients.Keys.ToList();

        return Result<List<Guid>>.Success(clients);
    }

    #region Events

    private async Task TerminateClient(IClient client, CancellationToken cancellationToken)
    {
        await client.Stop(cancellationToken);

        logger.LogInformation("[BrokerId: {brokerId}] Client stopping connection. First time.",
            client.BrokerId);

        _ = mediator.Publish(new TerminateEvent
        {
            Client = client,
        }, CancellationToken.None);
    }

    #endregion

    #region IDisposable

    public async ValueTask DisposeAsync()
    {
        logger?.LogInformation("Client Service ensuring graceful shutdown.");

        var writerLock = await _disposeLock.WriterLockAsync();

        try
        {
            if (serviceStopped)
                return;

            foreach (var (_, client) in _clients)
            {
                await client.Stop(CancellationToken.None);
                client.Dispose();
                logger?.LogInformation("[BrokerId: {brokerId}] Client disposed.",
                    client.BrokerId);
            }

            serviceStopped = true;
            _clients.Clear();
            GC.SuppressFinalize(this);
        }
        finally
        {
            writerLock.Dispose();
        }
    }

    #endregion
}
