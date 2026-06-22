using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Button;

internal static class ButtonWidgetExtensions
{
    public static ButtonWidget ToButtonWidget(this Widget widget, IOperation operation, ILogger<IWidget> logger)
    {
        var configuration = JsonSerializer
            .Deserialize<ButtonWidgetConfiguration>(widget.Configuration)!;

        return new ButtonWidget(widget, configuration, operation, logger);
    }
}
