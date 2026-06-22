using Senswave.Devices.Domain.Dashboards.Entities;

namespace Senswave.Devices.Application.Dashboards.Features.GetDashboard;

public class GetDashboardQuery : IQuery<Dashboard>
{
    public Guid UserId { get; set; }
    public Guid DashboardId { get; set; }
}
