using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateBroker;

public class CreateBrokerCommand : ICommand<Broker>
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int Port { get; set; }
    public BrokerProtocolVersion ProtocolVersion { get; set; }
    public bool UseTls { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
