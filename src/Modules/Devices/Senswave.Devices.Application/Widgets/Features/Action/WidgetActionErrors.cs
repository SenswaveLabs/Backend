namespace Senswave.Devices.Application.Widgets.Features.Action;

internal class WidgetActionErrors
{
    public static Error WidgetNotFound => Error.NotFound("WidgetNotFound", "Widget not found.");
    public static Error WidgetHasNoOperation => Error.NotFound("WidgetHasNoOperation", "Widget does not have an associated operation.");
}
