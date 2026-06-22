using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.Application.Brokers.Brokers.Features.UpdateBroker;

public class UpdateBrokerCommand : ICommand
{
    public Guid UserId { get; set; } = Guid.Empty;
    public Guid BrokerId { get; set; } = Guid.Empty;

    public string Name { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int Port { get; set; } = int.MaxValue;
    public BrokerProtocolVersion ProtocolVersion { get; set; } = BrokerProtocolVersion.Empty;
    public bool? UseTls { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
