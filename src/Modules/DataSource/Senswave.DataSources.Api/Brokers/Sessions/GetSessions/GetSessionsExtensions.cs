using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Api.Brokers.Sessions.GetSessions;

internal static class GetSessionsExtensions
{
    internal static GetSessionsResponse ToResponse(this Result<IEnumerable<Session>> result) => new()
    {
        Items = result.Data.Select(x => x.ToDto())
    };

    internal static SessionDto ToDto(this Session session) => new()
    {
        Id = session.Id,
        CreatedAtUtc = session.CreatedAtUtc,
        UpdatedAtUtc = session.UpdatedAtUtc,
        Finished = session.Finished,
    };
}
