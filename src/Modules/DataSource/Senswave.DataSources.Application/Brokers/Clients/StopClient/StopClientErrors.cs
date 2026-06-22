namespace Senswave.DataSources.Application.Brokers.Clients.StopClient;

internal class StartClientErrors
{
    public static Error BrokerNotFound => Error.NotFound("BrokerNotFound", "Broker not found.");
    public static Error ClientIsNotRunning => Error.Failure("ClientIsNotRunning", "Client is not running.");
    public static Error FailedToStopClient => Error.Failure("FailedToStopClient", "Failed to stop the client.");
}
