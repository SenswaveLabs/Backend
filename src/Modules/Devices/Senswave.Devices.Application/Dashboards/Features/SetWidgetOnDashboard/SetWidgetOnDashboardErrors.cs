namespace Senswave.Devices.Application.Dashboards.Features.SetWidgetOnDashboard;

internal class SetWidgetOnDashboardErrors
{
    internal static Error WidgetNotFound =
        Error.NotFound("WidgetNotFound", "Widget not found for device");

    internal static Error InvalidDeviceMembership =
        Error.Conflict("InvalidDeviceMembership", "Widget does not belong to any device on this dashboard.");
}
