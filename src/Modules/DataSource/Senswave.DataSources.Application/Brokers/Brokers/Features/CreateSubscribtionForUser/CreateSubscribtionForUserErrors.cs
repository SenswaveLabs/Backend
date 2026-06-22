namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtionForUser;

internal static class CreateSubscribtionForUserErrors
{
    public static Error BrokerNotFound => Error.NotFound("BrokerNotFound", "Broker was not found.");

    public static Error SubscribtionAlreadyExists => Error.Conflict("SubscribtionAlreadyExists", "Subscribtion already exists.");

    public static Error FailedToCreateSubscribtion => Error.ServerError("SubscribtionNotCreated", "Failed to create subscription.");

    public static Error MaximalNumberOfSubscribtions => Error.Conflict("MaximalNumberOfSubscribtions", "Maximal number of subscriptions per data source reached.");
}
