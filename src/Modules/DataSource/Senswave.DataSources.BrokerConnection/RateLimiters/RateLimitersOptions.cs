using Senswave.DataSources.BrokerConnection.RateLimiters.Message;
using Senswave.DataSources.Domain.Brokers.Clients.Options;

namespace Senswave.DataSources.BrokerConnection.RateLimiters;

public class RateLimitersOptions
{
    public const string SectionName = $"{ClientOptions.SectionName}:RateLimiters";

    public MessageRateLimiterOptions MessageRateLimiter { get; set; } = new();
}
