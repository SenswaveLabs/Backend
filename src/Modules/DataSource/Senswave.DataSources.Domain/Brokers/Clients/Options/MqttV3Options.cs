namespace Senswave.DataSources.Domain.Brokers.Clients.Options;

public class MqttV3Options
{
    public int PublishTimeoutMiliseconds { get; init; } = 1500;
    public int MessageTimeoutMiliseconds { get; init; } = 2 * 60 * 1000;
}
