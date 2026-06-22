using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Switch;

internal static class SwitchWidgetExtensions
{
    internal static SwitchWidget ToSwitchWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<SwitchWidgetConfiguration>(widget.Configuration)!;

        return new SwitchWidget(widget, configuration, operation, logger);
    }
}
