using Senswave.DataSources.Application.Brokers.Clients.StartClient;

namespace Senswave.DataSources.Api.Brokers.Clients.StartClient;

internal static class StartClientExtensions
{
    public static StartClientCommand ToCommand(this StartClientDto dto, Guid brokerId, Guid user) => new()
    {
        BrokerId = brokerId,
        UserId = user,
        Username = dto.Username,
        Password = dto.Password
    };
}
