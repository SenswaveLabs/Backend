namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteSubscribtion;

internal static class DeleteSubscribtionErrors
{
    public static Error BrokerNotFound
        => Error.NotFound("BrokerNotFound", "Broker was not found.");

    public static Error SubscribtionNotFound
        => Error.NotFound("SubscribtionNotFound", "Subscribtion was not found.");

    public static Error SubscribtionUsedByOperations
        => Error.Conflict("SubscribtionUsedByOperations", "Subscribtion is used by device operations and cannot be removed.");

    public static Error FailedToDeleteSubscribtion
        => Error.ServerError("FailedToDeleteSubscribtion", "Failed to delete subscribtion.");
}
