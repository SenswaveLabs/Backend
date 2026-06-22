using Senswave.Devices.Application.Widgets.Features.Action;

namespace Senswave.Devices.Api.Widgets.Features.Action;

internal static class WidgetActionExtensions
{
    public static WidgetActionCommand ToCommand(this WidgetActionRequest dto, Guid userId, Guid deviceId) => new()
    {
        WidgetId = deviceId,
        UserId = userId,
        Value = dto.Value
    };
}
