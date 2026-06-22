using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.TimeSeriesGraph;

internal static class TimeSeriesGraphWidgetExtensions
{
    internal static TimeSeriesGraphWidget ToGraphWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<TimeSeriesGraphWidgetConfiguration>(widget.Configuration)!;

        return new TimeSeriesGraphWidget(widget, operation, configuration, logger);
    }
}
