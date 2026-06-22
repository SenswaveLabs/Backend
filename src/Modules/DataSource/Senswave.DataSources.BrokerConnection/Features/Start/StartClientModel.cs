using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.BrokerConnection.Features.Start;

public class StartClientModel
{
    public Guid BrokerId { get; set; }
    public Guid SessionId { get; set; }

    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public BrokerProtocolVersion ProtocolVersion { get; set; }
    public bool UseTls { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public IList<string> Subscribtions { get; set; } = [];
}
