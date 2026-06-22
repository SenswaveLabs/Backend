using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Enums;
using Senswave.Devices.Domain.Dashboards.Models;
using Senswave.Devices.Domain.Dashboards.Types.Gird;

namespace Senswave.Devices.Domain.Dashboards.Types;

public abstract class BaseDashboard(Dashboard dashboard) : IDashboard
{
    protected readonly Dashboard _dashboard = dashboard;

    public Guid Id => _dashboard.Id;

    public Guid DeviceId => _dashboard.DeviceId;

    public string Name
    {
        get => _dashboard.Name;
        set => _dashboard.Name = value;
    }

    public string Icon
    {
        get => _dashboard.Icon;
        set => _dashboard.Icon = value;
    }

    public DateTime UpdatedAtUtc
    {
        get => _dashboard.UpdatedAtUtc;
        set => _dashboard.UpdatedAtUtc = value;
    }

    public DashboardType Type => _dashboard.Type;

    public DateTime CreatedAtUtc => _dashboard.CreatedAtUtc;

    public abstract Result AddWidget(PositionedWidget widget);

    public abstract Result RemoveWidget(Guid widgetId);

    public abstract Task<Result<DisplayDashboardModel>> ToDisplay(CancellationToken cancellationToken);

    public abstract Dashboard ToDashboard();

    public abstract Task<Result> Validate();
}
