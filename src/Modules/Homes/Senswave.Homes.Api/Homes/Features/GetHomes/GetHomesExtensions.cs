using Senswave.Abstractions.Resulting;
using Senswave.Homes.Api.Homes.Shared;
using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Api.Homes.Features.GetHomes;

internal static class GetHomesExtensions
{
    public static GetHomesResponse ToResponse(this Result<IEnumerable<Home>> result, Guid userId) => new()
    {
        Items = result.Data.Select(x => x.ToDto(userId)).ToList()
    };

    public static HomeDto ToDto(this Home home, Guid userId)
    {
        var homeDto = new HomeDto
        {
            Id = home.Id,
            DataSourceId = home.DataSourceReference?.DataSourceId,
            Name = home.Name,
            Icon = home.Icon,
            IsOwner = home.OwnerId == userId
        };

        if (home.Location != null)
            homeDto.Location = home.Location.ToDto();

        return homeDto;
    }
}