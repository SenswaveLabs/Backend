
namespace Senswave.DataSources.BrokerConnection.RateLimiters;

public interface IBrokerRateLimiter
{
    Task<bool> CanProcessMessage(Guid brokerId, Guid sessionId, string topic);
}
