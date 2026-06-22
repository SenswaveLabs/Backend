using Senswave.LiveUpdates.Api.Options;

namespace Senswave.LiveUpdates.Api;

public class LiveUpdatesOptions
{
    public const string SectionName = "Modules:LiveUpdates";

    public RateLimiterOptions RateLimiter { get; set; } = new();
}
