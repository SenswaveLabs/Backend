namespace Senswave.Api.Cors;

public class SenswaveWebsiteCorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = [];

    public bool Enabled { get; set; }
}
