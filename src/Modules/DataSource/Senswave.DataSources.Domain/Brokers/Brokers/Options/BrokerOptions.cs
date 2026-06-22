using Senswave.DataSources.Domain.Brokers.Clients.Options;

namespace Senswave.DataSources.Domain.Brokers.Brokers.Options;

public class BrokerOptions
{
    public const string SectionName = "Modules:DataSource:Brokers";

    public bool TestConnectionOnChange { get; set; } = true;
    public bool IsCluster { get; set; } = false;
    public ClientOptions Client { get; set; } = new();
    public LimitsOptions Limits { get; set; } = new();
}
