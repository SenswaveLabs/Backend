using Microsoft.Extensions.Options;
using Senswave.Abstractions.Resulting;
using System.Collections.Concurrent;

namespace Senswave.LiveUpdates.Api.Services.RateLimiter;

public class RateLimitingService(
    IOptions<LiveUpdatesOptions> options,
    ILogger<RateLimitingService> logger) : IRateLimiterService
{
    private Dictionary<string, ConcurrentQueue<DateTime>> Connections { get; } = [];

    public Task<Result> RateLimitingAllowsToWork(string connectionId)
    {
        var now = DateTime.UtcNow;

        var timeWindow = TimeSpan.FromSeconds(options.Value.RateLimiter.ReplenishmentPeriodSeconds);
        var maxTokens = options.Value.RateLimiter.TokensPerPeriod;

        if (!Connections.ContainsKey(connectionId))
        {
            var newQueue = new ConcurrentQueue<DateTime>();
            var added = Connections.TryAdd(connectionId, newQueue);

            if (added)
            {
                newQueue.Enqueue(now);
                return Task.FromResult(Result.Success());
            }
        }

        if (!Connections.TryGetValue(connectionId, out var timestamps))
        {
            logger.LogWarning("[Connection: {connectionId}] Rate limiter not found for connection.", connectionId);
            return Task.FromResult(Result.Failure(RateLimiterErrors.ConnectionNotFound));
        }

        while (timestamps.TryPeek(out var timestamp) && (now - timestamp) > timeWindow)
            timestamps.TryDequeue(out _);

        lock (timestamps)
        {
            if (timestamps.Count >= maxTokens)
            {
                logger.LogWarning("[Connection: {connectionId}] Rate limit exceeded. Current tokens: {currentTokens}, Max tokens: {maxTokens}",
                    connectionId, timestamps.Count, maxTokens);
                return Task.FromResult(Result.Failure(RateLimiterErrors.TooManyRequests));
            }

            timestamps.Enqueue(now);
        }

        return Task.FromResult(Result.Success());
    }
}
