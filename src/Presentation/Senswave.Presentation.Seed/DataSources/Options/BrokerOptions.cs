namespace Senswave.Presentation.Seed.DataSources.Options;

public class BrokerOptions
{
    public bool Create { get; set; } = false;

    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Port { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
