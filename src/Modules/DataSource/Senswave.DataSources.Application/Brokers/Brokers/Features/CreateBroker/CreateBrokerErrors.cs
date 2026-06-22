namespace Senswave.DataSources.Application.Brokers.Brokers.Features.CreateBroker;

public class CreateBrokerErrors
{
    public static Error MaximalNumberOfBrokers
        => Error.Conflict("MaximalNumberOfBrokers", "Maximal number of brokers reached.");

    public static Error CreationFailed
        => Error.ServerError("CreationFailed", "Failed to create broker.");

    public static Error NameAlreadyUsed
        => Error.Conflict("NameAlreadyUsed", "Name already used by other broker.");

    public static Error BrokerAlreadyUsed
        => Error.Conflict("BrokerAlreadyUsed", "Broker is already used.");

    public static Error ClientNameUsed
        => Error.Conflict("ClientNameUsed", "Client name is used for this broker, couldn't add new connection due to possible steal session problem.");

    public static Error MaximalNumberOfGlobalBrokers
        => Error.Conflict("MaximalNumberOfGlobalBrokers", "Maximal number of global brokers reached in service. Please contact support.");
}
