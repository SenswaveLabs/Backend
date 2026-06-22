namespace Senswave.Devices.Domain.Widgets.Types.TimeSeriesGraph;

public static class TimeSeriesDisplayTypeExtensions
{
    public static TimeSeriesDisplayType FromDisplayType(this string displayType) => displayType.ToLower() switch
    {
        "lines" => TimeSeriesDisplayType.Lines,
        "points" => TimeSeriesDisplayType.Points,
        "bars" => TimeSeriesDisplayType.Bars,
        "empty" => TimeSeriesDisplayType.Empty,
        _ => TimeSeriesDisplayType.Invalid
    };

    public static string ToDisplayTypeString(this TimeSeriesDisplayType displayType) => displayType switch
    {
        TimeSeriesDisplayType.Lines => "Lines",
        TimeSeriesDisplayType.Points => "Points",
        TimeSeriesDisplayType.Bars => "Bars",
        _ => "Invalid"
    };
}
