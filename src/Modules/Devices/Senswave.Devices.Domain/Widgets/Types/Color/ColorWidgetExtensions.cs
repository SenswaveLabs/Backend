using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Color;

internal static class ColorWidgetExtensions
{
    public static ColorWidget ToColorWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<ColorWidgetConfiguration>(widget.Configuration)!;

        return new ColorWidget(widget, configuration, operation, logger);
    }
}
