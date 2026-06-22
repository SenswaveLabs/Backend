using Senswave.Abstractions.Resulting;
using Senswave.DataSources.Domain.Brokers.Sessions.Entities;

namespace Senswave.DataSources.Api.Brokers.Sessions.GetSession;

internal static class GetSessionExtensions
{
    internal static GetSessionResponse ToResponse(this Result<Session> result) => new()
    {
        Id = result.Data.Id,
        CreatedAtUtc = result.Data.CreatedAtUtc,
        UpdatedAtUtc = result.Data.UpdatedAtUtc,
        Logs = result.Data.Logs.Select(x => x.ToDto())
    };

    internal static LogDto ToDto(this Log log) => new()
    {
        Id = log.Id,
        EventType = log.Type.ToString(),
        Data = log.Data,
        CreatedAtUtc = log.CreatedAtUtc
    };
}
