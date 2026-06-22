using Senswave.Abstractions.Resulting;

namespace Senswave.LiveUpdates.Api.Services.RateLimiter;

public interface IRateLimiterService
{
    Task<Result> RateLimitingAllowsToWork(string connectionId);
}
