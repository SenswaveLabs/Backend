namespace Senswave.Homes.Api.Homes.Shared;

public record LocationDto
{
    public double Longitude { get; init; }
    public double Latitude { get; init; }
}