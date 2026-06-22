using Senswave.DataSources.Domain.Brokers.Sessions.Entities;
using Senswave.DataSources.Domain.Brokers.Sessions.Enums;
using Senswave.DataSources.Domain.Brokers.Sessions.Repositories;
using Senswave.Integration.DataSource.BrokerConnection.Logging;
using System.Text.Json.Nodes;

namespace Senswave.DataSources.Infrastructure.Brokers.Clients.Features.ClientLogging;

public class MessageIgnoredLogHandler(
    ISessionCommandRepository repository,
    ILogger<MessageIgnoredLogHandler> logger) : IConsumer<MessageIgnoredLog>
{
    public async Task Consume(ConsumeContext<MessageIgnoredLog> context)
    {
        var result = await repository.CreateSessionLog(context.Message.SessionId, CreateLog(context.Message.Topic), context.CancellationToken);

        if (result)
            logger.LogInformation("[BrokerId: {brokerId}] [SessionId: {sessionId}] Stored information about ignoring receive message.",
                context.Message.BrokerId,
                context.Message.SessionId);
        else
            logger.LogError("[BrokerId: {brokerId}] [SessionId: {sessionId}] Failed to store information about ignoring receive message.",
                context.Message.BrokerId,
                context.Message.SessionId);

    }

    #region Privates

    private static Log CreateLog(string topic)
    {
        var jsonObject = new JsonObject()
        {
            ["Topic"] = topic
        };

        return new()
        {
            Type = SessionEventType.MessageIgnored,

            Data = jsonObject.ToJsonString(),

            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
    }

    #endregion
}
