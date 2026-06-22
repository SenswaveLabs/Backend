namespace Senswave.DataSources.Application.Brokers.Clients.StopClient;

public class StopClientCommand : ICommand
{
    public Guid BrokerId { get; set; }
    public Guid UserId { get; set; }
}
