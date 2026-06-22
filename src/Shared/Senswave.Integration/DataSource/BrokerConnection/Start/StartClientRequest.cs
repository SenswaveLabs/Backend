
namespace Senswave.Integration.DataSource.BrokerConnection.Start;

public record StartClientRequest
{
    public Guid BrokerId { get; set; }
    public Guid SessionId { get; set; }

    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public bool UseTls { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Subscribtions { get; set; } = [];
}
