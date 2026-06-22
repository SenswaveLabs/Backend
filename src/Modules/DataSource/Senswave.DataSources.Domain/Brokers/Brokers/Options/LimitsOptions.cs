namespace Senswave.DataSources.Domain.Brokers.Brokers.Options;

public class LimitsOptions
{
    public const string SectionName = $"{BrokerOptions.SectionName}:Limits";


    public int InstanceMaxBrokerClients { get; set; } = 5000;

    public int Brokers { get; set; } = 2;

    public int BrokerSubscribtions { get; set; } = 300;
}
