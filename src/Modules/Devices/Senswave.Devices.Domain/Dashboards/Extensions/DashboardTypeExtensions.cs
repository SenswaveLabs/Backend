using Senswave.Devices.Domain.Dashboards.Enums;

namespace Senswave.Devices.Domain.Dashboards.Extensions;

public static class DashboardTypeExtensions
{
    public static DashboardType ToDashboardType(this string type) => type.ToLowerInvariant() switch
    {
        "grid" => DashboardType.Grid,
        _ => DashboardType.Invalid
    };

    public static string FromDashboardType(this DashboardType type) => type switch
    {
        DashboardType.Grid => "grid",
        _ => ""
    };
}
