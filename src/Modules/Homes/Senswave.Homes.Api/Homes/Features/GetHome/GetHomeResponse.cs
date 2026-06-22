using Senswave.Homes.Api.Homes.Shared;

namespace Senswave.Homes.Api.Homes.Features.GetHome;

public class GetHomeResponse
{
    public Guid Id { get; init; }


    public string Name { get; init; } = string.Empty;

    public string Icon { get; init; } = string.Empty;

    public bool IsOwner { get; init; }


    public DataSourceDto? DataSource { get; set; }

    public LocationDto Location { get; set; } = new LocationDto();

    public ICollection<RoomDto> Rooms { get; set; } = [];
}