namespace Senswave.DataSources.Application.Brokers.Brokers.Features.DeleteBroker;

public static class DeleteBrokerConnectionErrors
{
    public static Error FailedToRemoveBroker
        => Error.ServerError("FailedToRemoveBroker", "Failed to remove broker from the database.");
    public static Error BrokerNotFound
        => Error.NotFound("BrokerNotFound", "The BrokerConnection with provided Id does not appear in the database");
    public static Error ClientExists
        => Error.Failure("ClientExists", "Client exists for this broker. First disconnect Client.");
    public static Error BrokerIsUsed
        => Error.Conflict("BrokerIsUsed", "Cannot remove broker used in homes.");
}