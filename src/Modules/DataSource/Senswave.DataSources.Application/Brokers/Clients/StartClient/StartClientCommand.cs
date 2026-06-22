namespace Senswave.DataSources.Application.Brokers.Clients.StartClient;

public class StartClientCommand : ICommand
{
    public Guid BrokerId { get; set; }
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
