namespace Senswave.Infrastructure.Diagnostics.OpenTelemetry;

public class OpenTelemetryOptions
{
    public const string SectionName = "Diagnostics:OpenTelemetry";

    public bool Enabled { get; set; } = false;
}
