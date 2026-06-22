using Senswave.Homes.Domain.Homes.ValueObjects;

namespace Senswave.Homes.Domain.Homes.Extensions;

public static class LocationExtensions
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Calculates the distance between the current Location and another Location in kilometers using Haversine formula.
    /// </summary>
    /// <param name="from">The starting Location (this instance).</param>
    /// <param name="to">The destination Location.</param>
    /// <returns>Distance in kilometers.</returns>
    public static double CalculateDistanceTo(this Location from, Location to) => CalculateDistanceTo(from, to.Latitude, to.Longitude);

    /// <summary>
    /// Calculates the distance between the current Location and another Location in kilometers using Haversine formula.
    /// </summary>
    /// <param name="from">The starting Location (this instance).</param>
    /// <param name="latitude">The destination latitude.</param>
    /// <param name="longitude">The destination longitude.</param>
    /// <returns>Distance in kilometers.</returns>
    public static double CalculateDistanceTo(this Location from, double latitude, double longitude)
    {
        double dLat = DegreesToRadians(latitude - from.Latitude);
        double dLon = DegreesToRadians(longitude - from.Longitude);

        double radLat1 = DegreesToRadians(from.Latitude);
        double radLat2 = DegreesToRadians(latitude);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(radLat1) * Math.Cos(radLat2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Degrees to be converted.</param>
    /// <returns>Radians equivalent of the input degrees.</returns>
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
