using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Slider;

internal static class SliderWidgetExtensions
{
    internal static SliderWidget ToSliderWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<SliderWidgetConfiguration>(widget.Configuration)!;

        return new SliderWidget(widget, configuration, operation, logger);
    }
}
