namespace Senswave.DataSources.Api.Brokers.Clients.GetClientState;

public class GetClientStateResponse
{
    public string ConnectionStatus { get; set; } = string.Empty;
    public Guid LatestSessionId { get; set; }
}
