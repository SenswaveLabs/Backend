using Senswave.Abstractions.ValueObjects;
using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.Domain.Brokers.Brokers.ValueObjects;

public class BrokerInfo : ValueObject
{
    public Guid BrokerId { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Url { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public string ClientName { get; set; } = string.Empty;

    public BrokerProtocolVersion ProtocolVersion { get; set; } = BrokerProtocolVersion.MqttV5;

    [Range(1, 65535)]
    public int Port { get; set; }

    public bool UseTls { get; set; }
}
