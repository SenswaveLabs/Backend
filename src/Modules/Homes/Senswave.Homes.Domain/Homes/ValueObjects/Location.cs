using Senswave.Homes.Domain.Homes.Entities;

namespace Senswave.Homes.Domain.Homes.ValueObjects;

public class Location
{
    public Home Home { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}