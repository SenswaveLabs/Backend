
namespace Senswave.DataSources.Application.Brokers.Clients.ClientState;

public record ClientStateModel
{
    public Domain.Brokers.Clients.Enums.ClientState State { get; set; } = Domain.Brokers.Clients.Enums.ClientState.NotStarted;
    public Guid LatestSession { get; set; } = Guid.Empty;
}
