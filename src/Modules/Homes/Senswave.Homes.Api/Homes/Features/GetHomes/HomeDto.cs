using Senswave.Homes.Api.Homes.Shared;

namespace Senswave.Homes.Api.Homes.Features.GetHomes;

public record HomeDto
{
    public Guid Id { get; init; }

    public Guid? DataSourceId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Icon { get; init; } = string.Empty;

    public bool IsOwner { get; init; }

    public LocationDto Location { get; set; } = new LocationDto();
}