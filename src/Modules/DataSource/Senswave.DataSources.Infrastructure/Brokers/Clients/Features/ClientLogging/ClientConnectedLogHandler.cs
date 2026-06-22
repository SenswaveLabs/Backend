using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Logging;
using Senswave.Integration.DataTransfer.DataSourceState;

namespace Senswave.DataSources.Infrastructure.Brokers.Clients.Features.ClientLogging;

public class ClientConnectedLogHandler(
    IPublishMessageBus bus,
    ISessionCommandRepository repository,
    ILogger<ClientConnectedLogHandler> logger) : IConsumer<ConnectedLog>
{
    public async Task Consume(ConsumeContext<ConnectedLog> context)
    {
        var result = await repository.CreateSessionLog(context.Message.SessionId, CreateLog(), context.CancellationToken);

        if (result)
            logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client connection stored.", context.Message.BrokerId, context.Message.SessionId);
        else
            logger.LogError("[BrokerId: {brokerId}] [SessionId: {sessionId}] Failed to store client connection log.", context.Message.BrokerId, context.Message.SessionId);

        var internalEvent = new DataSourceStateEvent
        {
            DataSourceId = context.Message.BrokerId,
            State = ClientState.Working.ToString()
        };

        await bus.Publish(internalEvent, context.CancellationToken);
    }

    #region Privates

    private static Log CreateLog() => new()
    {
        Type = SessionEventType.Connected,

        Data = "{}",

        CreatedAtUtc = DateTime.UtcNow,
        UpdatedAtUtc = DateTime.UtcNow,
    };

    #endregion
}
