using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;
using Senswave.Integration.DataSource.BrokerConnection.Start;

namespace Senswave.DataSources.BrokerConnection.Features.Start;

internal static class StartClientExtensions
{
    public static StartClientModel ToModel(this StartClientRequest request) => new()
    {
        SessionId = request.SessionId,
        BrokerId = request.BrokerId,

        Server = request.Server,
        ClientName = request.ClientName,
        Port = request.Port,
        ProtocolVersion = request.Protocol.ToProtocol(),
        UseTls = request.UseTls,

        Username = request.Username,
        Password = request.Password,

        Subscribtions = request.Subscribtions
    };
}
