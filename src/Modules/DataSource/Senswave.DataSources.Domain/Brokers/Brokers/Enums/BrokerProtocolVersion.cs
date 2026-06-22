namespace Senswave.DataSources.Domain.Brokers.Brokers.Enums;

public enum BrokerProtocolVersion
{
    Empty = -1,
    Invalid = 0,

    MqttV5 = 1,
    MqttV310 = 2,
    MqttV311 = 3,
}
