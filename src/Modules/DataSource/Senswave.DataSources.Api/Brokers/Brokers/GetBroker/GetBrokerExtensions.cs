using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;
using Senswave.DataSources.Domain.Brokers.Brokers.Extensions;

namespace Senswave.DataSources.Api.Brokers.Brokers.GetBroker;

internal static class GetBrokerExtensions
{
    internal static GetBrokerResponse ToBrokerResponse(this Result<Broker> result) => new()
    {
        Id = result.Data.Id,
        Name = result.Data.Name,

        Url = result.Data.BrokerInfo.Url,
        ClientName = result.Data.BrokerInfo.ClientName,
        Port = result.Data.BrokerInfo.Port,
        ProtocolVersion = result.Data.BrokerInfo.ProtocolVersion.FromProtocol(),
        UseTls = result.Data.BrokerInfo.UseTls,

        CreatedAt = result.Data.CreatedAtUtc,
        UpdatedAt = result.Data.UpdatedAtUtc
    };
}
