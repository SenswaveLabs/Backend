namespace Senswave.Devices.Application.Widgets.Features.State;

internal class StateErrors
{
    internal static Error WidgetNotFound = Error.Failure("WidgetNotFound", "Widget not found.");

    internal static Error FailedToUpdateWidget = Error.Failure("FailedToUpdateWidget", "Failed to update widget state.");
}
