using Senswave.Devices.Application.Widgets.Features.DeleteWidget;

namespace Senswave.Devices.Api.Widgets.Features.DeleteWidget;

internal static class DeleteWidgetExtensions
{
    internal static DeleteWidgetCommand ToDeleteWidgetCommand(this Guid widgetId, Guid userId) => new()
    {
        WidgetId = widgetId,
        OwnerId = userId
    };
}
