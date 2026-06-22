using Senswave.Abstractions.Resulting;
using Senswave.Homes.Application.Sharings.Features.GetSharings;
using Senswave.Homes.Domain.Sharings.Extensions;

namespace Senswave.Homes.Api.Sharings.GetSharings;

internal static class GetSharingsExtenstions
{
    internal static GetShraingsResponse ToResponse(this Result<IList<SharingModel>> result) => new()
    {
        Items = result.Data
                .Select(x => x.ToDto())
                .ToList()
    };

    public static SharingDto ToDto(this SharingModel model) => new()
    {
        SharingId = model.SharingId,
        SharingType = model.SharingType.FromHomeSharingType(),
        FriendEmail = model.FriendEmail
    };
}
