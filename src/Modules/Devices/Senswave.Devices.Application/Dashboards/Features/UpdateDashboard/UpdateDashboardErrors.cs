namespace Senswave.Devices.Application.Dashboards.Features.UpdateDashboard;

public static class UpdateDashboardErrors
{
    public static Error DashboardNotFound =>
        Error.NotFound("DashboardNotFound", "Dashboard with provided id not found in the database");

    public static Error DeviceNotFound =>
        Error.NotFound("DeviceNotFound", "Device with provided id not found in the database");

    public static Error DeviceWidgetConflict => Error.Conflict("DeviceWidgetConflict",
        "Can not change the device of the dashboard when it already has widget assigned");

    public static Error DashboardNameHasAlreadyExists => Error.Conflict("DashboardNameHasAlreadyExists",
        "Dashboard with provided 'name' has already exists in the system");

    public static Error DashboardBarError => Error.Conflict("DashboardBarError",
        "Dashboard can not shrink to provided 'columns' parameter because there is widget which position will be out of range");
}