namespace Senswave.DataSources.Application.Brokers.Clients.ClientState;

public class ClientStateQuery : IQuery<ClientStateModel>
{
    public Guid BrokerId { get; set; }
    public Guid UserId { get; set; }
}
