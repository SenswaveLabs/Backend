using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Senswave.DataSources.BrokerConnection.RateLimiters.Message;

public class MessageRateLimiter(
    ILogger<MessageRateLimiter> logger,
    IOptionsMonitor<RateLimitersOptions> options) : IBrokerRateLimiter
{
    private readonly ConcurrentQueue<DateTime> _processingTimes = new();

    private readonly Lock _cleanLock = new();

    private readonly Lock _writeLock = new();

    public Task<bool> CanProcessMessage(Guid brokerId, Guid sessionId, string topic)
    {
        var maxTokens = options.CurrentValue.MessageRateLimiter.TokenLimit;
        var timeWindow = TimeSpan.FromSeconds(options.CurrentValue.MessageRateLimiter.KeepTokenSeconds);
        var now = DateTime.UtcNow;

        lock (_cleanLock)
        {
            while (_processingTimes.TryPeek(out var timestamp) && (now - timestamp) > timeWindow)
            {
                _processingTimes.TryDequeue(out _);
            }
        }

        lock (_writeLock)
        {
            if (_processingTimes.Count >= maxTokens)
            {
                logger.LogWarning("Rate limit exceeded for broker {BrokerId}, session {SessionId}, topic {Topic}.", brokerId, sessionId, topic);
                return Task.FromResult(false);
            }

            _processingTimes.Enqueue(now);
        }

        return Task.FromResult(true);
    }
}
