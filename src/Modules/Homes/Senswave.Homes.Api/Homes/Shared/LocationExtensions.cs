using Senswave.Homes.Domain.Homes.ValueObjects;

namespace Senswave.Homes.Api.Homes.Shared;

internal static class LocationExtensions
{
    public static LocationDto ToDto(this Location location) => new()
    {
        Longitude = location.Longitude,
        Latitude = location.Latitude
    };
}
