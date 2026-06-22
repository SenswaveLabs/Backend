using Senswave.Integration.Shared;

namespace Senswave.Integration.DataSource.BrokerConnection.ClientState;

public record ClientStateResponse : BaseInternalResponse
{
    public int ClientState { get; set; } = 0;
}
