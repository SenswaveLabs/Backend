using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Domain.Brokers.Brokers.Entities;

namespace Senswave.DataSources.Api.Brokers.Brokers.GetSubscribtions;

internal static class GetSubscribtionsExtensions
{
    internal static GetSubscribtionsResponse ToResponse(this Result<IEnumerable<Subscribtion>> result) => new()
    {
        Items = result.Data.Select(x => x.ToDto())
    };

    internal static SubscribtionDto ToDto(this Subscribtion subscribtion) => new()
    {
        Id = subscribtion.Id,
        Topic = subscribtion.Topic,
        CreatedAt = subscribtion.CreatedAtUtc,
        UpdatedAt = subscribtion.UpdatedAtUtc
    };
}
