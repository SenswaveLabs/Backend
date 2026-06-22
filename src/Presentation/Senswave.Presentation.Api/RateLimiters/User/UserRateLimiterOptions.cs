namespace Senswave.Api.RateLimiters.User;

public class UserRateLimiterOptions
{
    public const string Name = $"{RateLimitersOptions.Name}:User:Token";

    public int QueueLimit { get; set; } = 0;
    public int TokensPerPeriod { get; set; } = 20;
    public int TokenLimit { get; set; } = 100;
    public int ReplenishmentPeriodSeconds { get; set; } = 30;
    public TimeSpan ReplenishmentPeriod => TimeSpan.FromSeconds(ReplenishmentPeriodSeconds);
}
