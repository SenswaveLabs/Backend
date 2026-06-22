namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateSubscribtion;

internal class CreateSubscribtionErrors
{
    public static Error BrokerNotFound = Error.Failure("BrokerNotFound", "Broker was not found.");

    public static Error SubscribtionAlreadyExists = Error.Failure("SubscribtionAlreadyExists", "Subscribtion already exists.");

    public static Error FailedToCreateSubscribtion = Error.ServerError("SubscribtionNotCreaded", "Failed to create subscription.");

    public static Error MaximalNumberOfSubscribtions = Error.Conflict("MaximalNumberOfSubscribtions", "Maximal number of subscriptions per data source reached.");
}
