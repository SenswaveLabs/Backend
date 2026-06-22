using Senswave.DataSources.Domain.Brokers.Brokers.Options;

namespace Senswave.DataSources.Domain.Brokers.Clients.Options;

public class ClientOptions
{
    public const string SectionName = $"{BrokerOptions.SectionName}:Client";

    public int MilisecondSpanBetweenMessagesOnSameTopic { get; set; } = 500;
    public int MaximalSizeOfMessageInBytes { get; set; } = 4096;

    public int ReconnectAttempts { get; set; } = 5;
    public int ReconnectMilisecondSpanBetweenAttempts { get; set; } = 10000;

    public MqttV3Options MqttV3 { get; set; } = new();

    public ClientCleanupOptions ClientCleanup { get; set; } = new();
}
