namespace Senswave.DataSources.Domain.Brokers.Clients.Options;

public class ClientCleanupOptions
{
    public const string SectionName = $"{ClientOptions.SectionName}:Cleanup";

    public bool Enabled { get; set; } = true;
    public double CleanupSpanMinutes { get; set; } = 60;
    public int ClientCleanupSpanMiliseconds { get; set; } = 120000;
}
