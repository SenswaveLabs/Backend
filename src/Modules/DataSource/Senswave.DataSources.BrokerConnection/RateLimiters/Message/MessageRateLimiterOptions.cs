namespace Senswave.DataSources.BrokerConnection.RateLimiters.Message;

public class MessageRateLimiterOptions
{
    public const string SectionName = $"{RateLimitersOptions.SectionName}:MessageRateLimiter";

    public int TokenLimit { get; set; } = 150;

    public int KeepTokenSeconds { get; set; } = 60;
}
