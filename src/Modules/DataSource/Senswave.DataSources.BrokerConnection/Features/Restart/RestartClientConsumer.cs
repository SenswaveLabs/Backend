using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.Restart;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.BrokerConnection.Features.Restart;

public class RestartClientConsumer(
    IClientService clientService,
    ILogger<RestartClientConsumer> logger) : IConsumer<RestartClientRequest>
{
    public async Task Consume(ConsumeContext<RestartClientRequest> context)
    {
        var restart = await clientService.RestartClient(context.Message.BrokerId, context.CancellationToken);

        var response = new RestartClientResponse
        {
            StatusCode = restart.IsSuccess ? InternalRequestStatus.Success : InternalRequestStatus.Failure,
            Error = (Error)(restart.IsSuccess ? Error.None : restart.Errors[0])
        };

        if (restart.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client restarted successfully.", context.Message.BrokerId);
        }
        else
        {
            logger.LogError("[Broker: {brokerId}] Failed to restart client.", context.Message.BrokerId);
        }

        await context.RespondAsync(response);
    }
}
