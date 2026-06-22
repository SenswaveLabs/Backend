using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Application.Brokers.Clients.ClientState;

namespace Senswave.DataSources.Api.Brokers.Clients.GetClientState;

internal static class GetClientStateExtensions
{
    internal static GetClientStateResponse ToClientStateResponse(this Result<ClientStateModel> result) => new()
    {
        ConnectionStatus = result.Data.State.ToString(),
        LatestSessionId = result.Data.LatestSession,
    };
}
