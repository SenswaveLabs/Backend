using Senswave.DataSources.Domain.Brokers.Brokers.Enums;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Extensions;

public static class BrokerProtocolVersionExtensions
{
    public static bool IsValidForNetClient(this BrokerProtocolVersion version) => version switch
    {
        BrokerProtocolVersion.MqttV310 or
        BrokerProtocolVersion.MqttV311 or
        BrokerProtocolVersion.MqttV5 => true,
        _ => false,
    };

    public static BrokerProtocolVersion ToProtocol(this string protocol) => protocol switch
    {
        "MqttV5" => BrokerProtocolVersion.MqttV5,
        "MqttV310" => BrokerProtocolVersion.MqttV310,
        "MqttV311" => BrokerProtocolVersion.MqttV311,
        "" => BrokerProtocolVersion.Empty,
        _ => BrokerProtocolVersion.Invalid
    };

    public static string FromProtocol(this BrokerProtocolVersion protocol) => protocol switch
    {
        BrokerProtocolVersion.MqttV5 => "MqttV5",
        BrokerProtocolVersion.MqttV310 => "MqttV310",
        BrokerProtocolVersion.MqttV311 => "MqttV311",
        _ => "",
    };
}
