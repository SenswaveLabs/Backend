using Senswave.Abstractions.Resulting;
using Senswave.Devices.Application.Dashboards.Features.CreateDashboard;
using Senswave.Devices.Domain.Dashboards.Entities;

namespace Senswave.Devices.Api.Dashboards.CreateDashboard;

internal static class CreateDashboardExtensions
{
    internal static DashboardCreatedResponse ToCreatedResponse(this Result<Dashboard> result) => new()
    {
        Id = result.Data.Id
    };

    public static CreateDashboardCommand ToCommand(this CreateDashboardRequest request, Guid userId) => new()
    {
        UserId = userId,
        DeviceId = request.DeviceId,

        Name = request.Name,
        Icon = request.Icon,

        Configuration = request.Configuration
    };
}
