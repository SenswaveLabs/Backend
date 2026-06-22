namespace Senswave.Devices.Application.Dashboards.Features.CreateDashboard;

public class CreateDashboardErrors
{
    public static Error DeviceNotFound
        => Error.NotFound("DeviceNotFound", "Device with provided id not found");

    public static Error DashboardAlreadyCreated
        => Error.Conflict("DashboardAlreadyCreated", "Dashboard with provided name already exists in database for provided device");

    public static Error DashboardsLimitReached
        => Error.Conflict("DashboardsLimitReached", "Limit of dashboards per device reached");
}