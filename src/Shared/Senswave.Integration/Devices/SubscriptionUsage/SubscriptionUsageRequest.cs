namespace Senswave.Integration.Devices.SubscriptionUsage;

public record SubscriptionUsageRequest
{
    public Guid SubscriptionId { get; init; } = Guid.Empty;
}
