namespace Senswave.Integration.DataSource.CreateSubscription;

/// <summary>
/// Message sending to DataSource module to create subscription
/// </summary>
public record CreateSubscriptionRequest
{
    public Guid BrokerId { get; set; } = Guid.Empty;

    public string Topic { get; set; } = string.Empty;
}