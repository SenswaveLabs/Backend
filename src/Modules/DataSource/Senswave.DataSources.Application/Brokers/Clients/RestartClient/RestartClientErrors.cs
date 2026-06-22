namespace Senswave.DataSources.Application.Brokers.Clients.RestartClient;

internal static class RestartClientErrors
{
    public static Error BrokerNotFound => Error.NotFound("BrokerNotFound", "Broker not found.");
    public static Error FailedToRestartClient => Error.NotFound("FailedToRestartClient", "Failed to restart the client.");
}
