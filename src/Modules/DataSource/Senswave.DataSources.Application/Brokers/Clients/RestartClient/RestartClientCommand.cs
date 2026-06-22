namespace Senswave.DataSources.Application.Brokers.Clients.RestartClient;

public class RestartClientCommand : ICommand
{
    public Guid BrokerId { get; set; }
    public Guid UserId { get; set; }
}
