using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace Senswave.Api.RateLimiters.Anonymous;

public class AnonymousRateLimiter(IOptions<AnonymousRateLimiterOptions> unknowUserOptions,
    ILogger<AnonymousRateLimiter> logger)
    : IRateLimiterPolicy<string>
{
    public const string PolicyName = "AnonymousTokenRateLimiter";

    private const string AnonymousPartitionKey = "AnonymousTokenPartitionKey";

    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected
        => RateLimiterExtensions.AcceptRejectRule;

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        // X-Forwarded-For used by Reverse Proxy
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            return RateLimitPartition.GetTokenBucketLimiter(forwardedFor.ToString(), _ => GetOptions(1));

        var ip = httpContext.Connection.RemoteIpAddress?.ToString();

        if (!string.IsNullOrEmpty(ip))
            return RateLimitPartition.GetTokenBucketLimiter(ip, _ => GetOptions(1));

        logger.LogWarning("[RquestId: {RequestId}] [AnonymousRateLimiter] Failed to get IP address for rate limiting.", httpContext.TraceIdentifier);

        return RateLimitPartition.GetTokenBucketLimiter(AnonymousPartitionKey, _ => GetOptions(5));
    }

    #region Privates

    private TokenBucketRateLimiterOptions GetOptions(int factor) => new()
    {
        QueueLimit = factor * unknowUserOptions.Value.QueueLimit,
        ReplenishmentPeriod = factor * unknowUserOptions.Value.ReplenishmentPeriod,
        TokenLimit = factor * unknowUserOptions.Value.TokenLimit,
        TokensPerPeriod = factor * unknowUserOptions.Value.TokensPerPeriod,

        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        AutoReplenishment = true
    };

    #endregion
}