using Senswave.Devices.Application.Widgets.Features.State;

namespace Senswave.Devices.Api.Widgets.Features.State;

internal static class StateExtensions
{
    internal static StateCommand ToCommand(this StateRequest request, IRequestContext context, Guid widgetId) => new()
    {
        Enabled = request.Enabled,
        UserId = context.UserId,
        WidgetId = widgetId
    };
}
