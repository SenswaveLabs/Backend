namespace Senswave.LiveUpdates.Api.Options;

public class RateLimiterOptions
{
    public const string SectionName = $"{LiveUpdatesOptions.SectionName}:RateLimiter";

    public int TokensPerPeriod { get; set; } = 5;

    public int ReplenishmentPeriodSeconds { get; set; } = 30;
}
