using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.ClientExists;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.BrokerConnection.Features.ClientExists;

public class ClientExistsConsumer(
    IClientService clientService,
    ILogger<ClientExistsConsumer> logger) : IConsumer<ClientExistsRequest>
{
    #region Message

    public static ClientExistsResponse ClientExist => new()
    {
        StatusCode = InternalRequestStatus.Success
    };

    public static ClientExistsResponse ClientDoesNotExist => new()
    {
        StatusCode = InternalRequestStatus.Failure
    };

    #endregion

    public async Task Consume(ConsumeContext<ClientExistsRequest> context)
    {
        var client = clientService.GetClient(context.Message.BrokerId);

        var response = client.IsSuccess ? ClientExist : ClientDoesNotExist;

        if (client.IsSuccess)
        {
            logger.LogInformation("[Broker: {brokerId}] Client exists.", context.Message.BrokerId);
        }
        else
        {
            logger.LogWarning("[Broker: {brokerId}] Client does not exist.", context.Message.BrokerId);
        }

        await context.RespondAsync(response);
    }
}
