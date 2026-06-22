using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Api.Brokers.Brokers.GetBrokers;

internal static class GetBrokerExtensions
{
    internal static GetBrokersResponse ToBrokersResponse(this Result<IEnumerable<Broker>> result) => new()
    {
        Items = result.Data
            .Select(x => x.ToDto())
    };

    internal static BrokerDto ToDto(this Broker broker) => new()
    {
        Id = broker.Id,
        Name = broker.Name,
        Server = $"{broker.BrokerInfo.Url}:{broker.BrokerInfo.Port}",

        CreatedAt = broker.CreatedAtUtc,
        UpdatedAt = broker.UpdatedAtUtc
    };
}
