namespace Senswave.Integration.DataSource.CreateSubscription;

using Senswave.Integration.Shared;

public record CreateSubscriptionResponse : BaseInternalResponse
{
    public Guid SubscriptionId { get; set; } = Guid.Empty;
}