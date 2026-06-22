using Senswave.Devices.Domain.Operations.Factory;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Radio;

internal static class RadioWidgetExtensions
{
    internal static RadioWidget ToRadioWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<RadioWidgetConfiguration>(widget.Configuration)!;

        return new RadioWidget(widget, configuration, operation, logger);
    }
}
