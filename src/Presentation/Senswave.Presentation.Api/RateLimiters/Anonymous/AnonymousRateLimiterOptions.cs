namespace Senswave.Api.RateLimiters.Anonymous;

public class AnonymousRateLimiterOptions
{
    public const string Name = $"{RateLimitersOptions.Name}:Anonymous:Token";

    public int QueueLimit { get; set; } = 0;
    public int TokensPerPeriod { get; set; } = 5;
    public int TokenLimit { get; set; } = 15;
    public int ReplenishmentPeriodSeconds { get; set; } = 30;
    public TimeSpan ReplenishmentPeriod => TimeSpan.FromSeconds(ReplenishmentPeriodSeconds);
}
