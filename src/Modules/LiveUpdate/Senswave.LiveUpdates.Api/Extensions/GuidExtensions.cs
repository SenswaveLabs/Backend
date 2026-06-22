namespace Senswave.LiveUpdates.Api.Extensions;

public static class GuidExtensions
{
    public static string ToHomesGroupName(this Guid homesId)
        => $"senswave-homes-{homesId}";

    public static string ToDevicesGroupName(this Guid deviceId)
        => $"senswave-devices-{deviceId}";

    public static string ToDataSourcesGroupName(this Guid dataSourceId)
        => $"senswave-datasources-{dataSourceId}";
}
