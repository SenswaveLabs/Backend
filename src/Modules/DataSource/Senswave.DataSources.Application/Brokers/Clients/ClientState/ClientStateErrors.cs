namespace Senswave.DataSources.Application.Brokers.Clients.ClientState;

internal sealed class ClientStateErrors
{
    public static Error BrokerNotFound => Error.NotFound("BrokerNotFound", "Broker not found.");
    public static Error FailedToGetStatus => Error.NotFound("FailedToGetStatus", "Failed to retrieve client status.");
}
