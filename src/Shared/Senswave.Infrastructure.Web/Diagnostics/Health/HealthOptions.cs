namespace Senswave.Infrastructure.Web.Diagnostics.Health;

public class HealthOptions
{
    public const string SectionName = "Diagnostics:Health";

    public string MainPortAccessToken { get; set; } = string.Empty;

    public string MainPortHeaderName { get; set; } = "X-API-KEY";

    public string Path { get; set; } = "health";

    public int Port { get; set; } = 5001;

    public int HealthCheckTimeoutSeconds { get; set; } = 10;
}
