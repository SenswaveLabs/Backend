using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace Senswave.Api.RateLimiters.User;

public class UserRateLimiter(IOptions<UserRateLimiterOptions> userOptions)
    : IRateLimiterPolicy<string>
{
    public const string PolicyName = "UserTokenRateLimiter";
    private const string UserPartitionKey = "UserTokenPartitionKey";

    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected
        => RateLimiterExtensions.AcceptRejectRule;

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var partitioningKey = httpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(partitioningKey))
            partitioningKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? UserPartitionKey;

        return RateLimitPartition.GetTokenBucketLimiter(partitioningKey, _ => GetOptions());
    }

    #region Privates

    private TokenBucketRateLimiterOptions GetOptions() => new()
    {
        TokenLimit = userOptions.Value.TokenLimit,
        QueueLimit = userOptions.Value.QueueLimit,
        ReplenishmentPeriod = userOptions.Value.ReplenishmentPeriod,
        TokensPerPeriod = userOptions.Value.TokensPerPeriod,

        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        AutoReplenishment = true
    };

    #endregion
}
