using Senswave.DataSources.Application.Brokers.Clients.StopClient;

namespace Senswave.DataSources.Api.Brokers.Clients.StopClient;

internal static class StopClientExtensions
{
    public static StopClientCommand ToStopClientCommand(this Guid brokerId, Guid userId) => new()
    {
        BrokerId = brokerId,
        UserId = userId,
    };
}
