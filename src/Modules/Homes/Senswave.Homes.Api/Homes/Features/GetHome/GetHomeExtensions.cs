using Senswave.Abstractions.Resulting;
using Senswave.Homes.Api.Homes.Shared;
using Senswave.Homes.Application.Homes.Models;
using Senswave.Homes.Domain.Rooms.Entities;

namespace Senswave.Homes.Api.Homes.Features.GetHome;

public static class GetHomeExtensionsHomeExtensions
{
    public static GetHomeResponse ToHomeResponse(this Result<GetHomeModel> result, Guid userId)
    {
        var homeDto = new GetHomeResponse
        {
            Id = result.Data.Home.Id,

            Name = result.Data.Home.Name,
            Icon = result.Data.Home.Icon,
            IsOwner = result.Data.Home.OwnerId == userId,
        };

        if (result.Data.Home.DataSourceReference is not null)
        {
            homeDto.DataSource = new DataSourceDto()
            {
                Id = result.Data.Home.DataSourceReference?.DataSourceId,
                State = result.Data.DataSource!.State,
                Name = result.Data.DataSource!.Name
            };
        }

        if (result.Data.Home.Location is not null)
        {
            homeDto.Location = result.Data.Home.Location
                .ToDto();
        }

        if (result.Data.Home.Rooms.Any())
        {
            homeDto.Rooms = result.Data.Home.Rooms
                .Select(x => x.ToDto())
                .ToList();
        }

        return homeDto;
    }

    public static RoomDto ToDto(this Room room) => new()
    {
        Id = room.Id,
        Name = room.Name
    };
}