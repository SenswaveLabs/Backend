using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.Stop;
using Senswave.Integration.Shared;


namespace Senswave.DataSources.BrokerConnection.Features.Stop;

public class StopClientConsumer(
    IClientService clientService,
    ILogger<StopClientConsumer> logger) : IConsumer<StopClientRequest>
{
    #region Error

    private static readonly Error FailedToStart = Error.Failure("FailedToStart", "Failed to stop the client.");

    #endregion

    public async Task Consume(ConsumeContext<StopClientRequest> context)
    {
        var result = await clientService.StopClient(context.Message.BrokerId, context.CancellationToken);

        var response = result.IsSuccess ? Success(result.Data) : Failure(result.Errors);

        if (result.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client stopped successfully.", context.Message.BrokerId);
        }
        else
        {
            logger.LogError("[Broker: {brokerId}] Failed to stop client", context.Message.BrokerId);
        }

        await context.RespondAsync(response);
    }

    #region Messages

    public static StopClientResponse Failure(Error[] errors) => new()
    {
        StatusCode = InternalRequestStatus.Failure,
        Error = errors.Length > 0 ? (Error)errors[0] : FailedToStart
    };

    public static StopClientResponse Success(Guid sessionId) => new()
    {
        StatusCode = InternalRequestStatus.Success,
        SessionId = sessionId
    };

    #endregion
}
