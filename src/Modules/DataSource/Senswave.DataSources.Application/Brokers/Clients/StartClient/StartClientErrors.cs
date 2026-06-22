namespace Senswave.DataSources.Application.Brokers.Clients.StartClient;

internal class StartClientErrors
{
    public static Error FailedToEndOldSessions => Error.ServerError("FailedToEndOldSessions", "Failed to end old sessions before starting a new one.");
    public static Error BrokerNotFound => Error.NotFound("BrokerNotFound", "Broker not found.");
    public static Error ClientIsRunning => Error.ServerError("ClientIsRunning", "First stop client.");
    public static Error FailedToStartSession => Error.Failure("FailedToStartSession", "Failed to start the broker session.");
}
