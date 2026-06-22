namespace Senswave.DataSources.Api.Brokers.Brokers.CreateBroker;

public record CreateBrokerRequest
{
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int Port { get; set; }
    public string ProtocolVersion { get; set; } = string.Empty;
    public bool UseTls { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
