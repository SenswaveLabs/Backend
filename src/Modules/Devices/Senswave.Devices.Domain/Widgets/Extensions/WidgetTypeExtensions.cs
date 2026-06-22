using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.Domain.Widgets.Extensions;

public static class WidgetTypeExtensions
{
    public static WidgetType ToWidgetType(this string type) => type.ToLowerInvariant() switch
    {
        "button" => WidgetType.Button,
        "display" => WidgetType.Display,
        "slider" => WidgetType.Slider,
        "switch" => WidgetType.Switch,
        "radio" => WidgetType.Radio,
        "color" => WidgetType.Color,
        "timeseriesgraph" => WidgetType.TimeSeriesGraph,

        "" => WidgetType.Empty,
        _ => WidgetType.Invalid
    };

    public static string FromWidgetType(this WidgetType type) => type switch
    {
        WidgetType.Button => "Button",
        WidgetType.Display => "Display",
        WidgetType.Slider => "Slider",
        WidgetType.Switch => "Switch",
        WidgetType.Radio => "Radio",
        WidgetType.Color => "Color",
        WidgetType.TimeSeriesGraph => "TimeSeriesGraph",

        _ => ""
    };
}
