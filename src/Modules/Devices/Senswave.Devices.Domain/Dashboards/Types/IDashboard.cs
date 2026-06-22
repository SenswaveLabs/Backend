using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Models;
using Senswave.Devices.Domain.Dashboards.Types.Gird;

namespace Senswave.Devices.Domain.Dashboards.Types;

public interface IDashboard
{
    //TODO: Create Separate Inteface when new dashboard Type
    Result AddWidget(PositionedWidget widget);

    Result RemoveWidget(Guid widgetId);

    Task<Result<DisplayDashboardModel>> ToDisplay(CancellationToken cancellationToken);

    Dashboard ToDashboard();

    Task<Result> Validate();
}
