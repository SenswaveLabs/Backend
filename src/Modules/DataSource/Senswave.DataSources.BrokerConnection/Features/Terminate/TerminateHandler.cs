using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Events;
using Senswave.DataSources.Domain.Diagnostics;
using System.Diagnostics;

namespace Senswave.DataSources.BrokerConnection.Features.Terminate;

public class TerminateHandler(
    IDataSourcesActivityProvider activityProvider,
    ILogger<TerminateHandler> logger) : IModuleEventHandler<TerminateEvent>
{
    public async Task Handle(TerminateEvent notification, CancellationToken cancellationToken)
    {
        Activity.Current = null;
        using var activity = activityProvider.StartActivity("Mqtt /terminate");
        activity?.AddTag("mqtt.client.broker_id", notification.Client?.BrokerId);
        activity?.AddTag("mqtt.client.session_id", notification.Client?.SessionId);

        if (notification.Client is null)
        {
            logger.LogWarning("[BrokerId: {brokerId}][SessionId: {sessionId}] Client is null, cannot terminate session.",
                notification.Client?.BrokerId,
                notification.Client?.SessionId);

            activity?.AddEvent(new("Client already terminated"));
            return;
        }

        logger.LogInformation("[BrokerId: {brokerId}][SessionId: {sessionId}] Terminating session.",
            notification.Client.BrokerId,
            notification.Client.SessionId);

        await notification.Client.Stop(cancellationToken);
        await Task.Delay(60000, CancellationToken.None);
        await notification.Client.Stop(CancellationToken.None);
        notification.Client.Dispose();
        activity?.AddEvent(new("Client terminated."));

        logger.LogInformation("[BrokerId: {brokerId}][SessionId: {sessionId}] Session terminated.",
            notification.Client.BrokerId,
            notification.Client.SessionId);
    }
}
