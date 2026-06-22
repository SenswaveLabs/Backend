using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Display;

internal static class DisplayWidgetExtensions
{
    internal static DisplayWidget ToDisplayWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<DisplayWidgetConfiguration>(widget.Configuration)!;

        return new DisplayWidget(widget, operation, configuration, logger);
    }
}
