namespace Senswave.DataSources.Application.Brokers.Brokers.Features.GetSubscribtions;

public static class GetSubscribtionsErrors
{
    public static Error BrokerNotFound => Error.NotFound("BrokerNotFound", "Broker not found.");
    public static Error SubscribtionsNotFound => Error.NotFound("SubscribtionsNotFound", "No subscriptions found.");
}
