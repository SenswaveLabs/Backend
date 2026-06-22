using Senswave.DataSources.Application.Brokers.Clients.RestartClient;

namespace Senswave.DataSources.Api.Brokers.Clients.RestartClient;

internal static class RestartClientExtensions
{
    internal static RestartClientCommand ToRestartClientCommand(this Guid brokerId, Guid userId) => new()
    {
        BrokerId = brokerId,
        UserId = userId,
    };
}
