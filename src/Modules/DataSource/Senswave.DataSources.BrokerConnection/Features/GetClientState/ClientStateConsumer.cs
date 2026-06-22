using MassTransit;
using Microsoft.Extensions.Logging;
using Senswave.Abstractions.Resulting;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Domain.Brokers.Clients.Entities;
using Senswave.DataSources.Domain.Brokers.Clients.Enums;
using Senswave.Integration.DataSource.BrokerConnection.ClientState;
using Senswave.Integration.Shared;

namespace Senswave.DataSources.BrokerConnection.Features.GetClientState;

public class ClientStateConsumer(IClientService clientService, ILogger<ClientStateConsumer> logger) : IConsumer<ClientStateRequest>
{
    public async Task Consume(ConsumeContext<ClientStateRequest> context)
    {
        var clientResult = clientService.GetClient(context.Message.BrokerId);

        var response = GetResponseMessage(clientResult);

        logger.LogInformation("[BrokerId: {brokerId}] Deduced client state: {state}", context.Message.BrokerId, response.ClientState);

        await context.RespondAsync(response);
    }

    #region Privates

    private ClientStateResponse GetResponseMessage(Result<IClient> clientResult)
    {
        if (clientResult.IsFailure)
            return new ClientStateResponse { ClientState = (int)ClientState.NotStarted, StatusCode = InternalRequestStatus.Success };

        if (clientResult.Data.IsConnected)
            return new ClientStateResponse { ClientState = (int)ClientState.Working, StatusCode = InternalRequestStatus.Success };

        if (clientResult.Data.Remove)
            return new ClientStateResponse { ClientState = (int)ClientState.Stopped, StatusCode = InternalRequestStatus.Success };

        return new ClientStateResponse { ClientState = (int)ClientState.Restarting, StatusCode = InternalRequestStatus.Success };
    }

    #endregion
}
