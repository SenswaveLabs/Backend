using Senswave.Devices.Domain.Dashboards.Models;

namespace Senswave.Devices.Application.Dashboards.Features.DisplayDashboard;

public class DisplayDashboardQuery : IQuery<DisplayDashboardModel>
{
    public Guid UserId { get; set; }

    public Guid DashboardId { get; set; }
}
