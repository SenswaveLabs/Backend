using Senswave.Devices.Domain.Dashboards.Entities;

namespace Senswave.Devices.Application.Dashboards.Features.DisplayDashboards;

public class DisplayDashboardsQuery : IQuery<List<Dashboard>>
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}
