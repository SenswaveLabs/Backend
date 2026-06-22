using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.Start;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.BrokerConnection.Features.Start;

public class StartClientConsumer(
    IClientService clientService,
    ILogger<StartClientConsumer> logger) : IConsumer<StartClientRequest>
{
    #region Error

    private static readonly Error FailedToStart = Error.Failure("FailedToStart", "Failed to start the client.");

    #endregion

    public async Task Consume(ConsumeContext<StartClientRequest> context)
    {
        var model = context.Message.ToModel();

        var result = await clientService.StartClient(model, context.CancellationToken);

        var response = result.IsSuccess ? Success() : Failure(result.Errors);
        if (result.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client started successfully.", context.Message.BrokerId);
        }
        else
        {
            logger.LogError("[Broker: {brokerId}] Failed to start client.", context.Message.BrokerId);
        }

        await context.RespondAsync(response);
    }

    #region Messages

    public static StartClientResponse Failure(Error[] errors) => new()
    {
        StatusCode = InternalRequestStatus.Failure,
        Error = errors.Length > 0 ? (Error)errors[0] : FailedToStart,
    };


    public static StartClientResponse Success() => new()
    {
        StatusCode = InternalRequestStatus.Success
    };

    #endregion
}
