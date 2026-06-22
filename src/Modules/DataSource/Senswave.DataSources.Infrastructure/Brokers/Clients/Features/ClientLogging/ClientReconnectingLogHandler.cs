using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Logging;
using Senswave.Integration.DataTransfer.DataSourceState;
using System.Text.Json.Nodes;

namespace Senswave.DataSources.Infrastructure.Brokers.Clients.Features.ClientLogging;

internal class ClientReconnectingLogHandler(
    IPublishMessageBus bus,
    ISessionCommandRepository repository,
    ILogger<ClientReconnectingLogHandler> logger)
    : IConsumer<ReconnectingLog>
{
    public async Task Consume(ConsumeContext<ReconnectingLog> context)
    {
        var result = await repository.CreateSessionLog(context.Message.SessionId, CreateLog(context.Message.Reason), context.CancellationToken);

        if (result)
            logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Client reconnecting stored.", context.Message.BrokerId, context.Message.SessionId);
        else
            logger.LogError("[BrokerId: {brokerId}] [SessionId: {sessionId}] Failed to store client reconnecting log.",
                context.Message.BrokerId,
                context.Message.SessionId);

        var internalEvent = new DataSourceStateEvent
        {
            DataSourceId = context.Message.BrokerId,
            State = ClientState.Restarting.ToString()
        };

        await bus.Publish(internalEvent, context.CancellationToken);
    }

    #region Privates

    private static Log CreateLog(string reason)
    {
        var jsonObject = new JsonObject()
        {
            ["Reason"] = reason
        };

        return new()
        {
            Type = SessionEventType.Reconnecting,
            Data = jsonObject.ToJsonString(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    #endregion
}