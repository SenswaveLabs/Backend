using Senswave.Devices.Application.Dashboards.Features.SetWidgetOnDashboard;

namespace Senswave.Devices.Api.Dashboards.SetWidgetOnDashboard;

internal static class SetWidgetOnDashboardExtensions
{
    internal static SetWidgetOnDashboardCommand ToSetWidgetCommand(this SetWidgetOnDashboardRequest request, Guid userId, Guid dashboardId) => new()
    {
        UserId = userId,
        DashboardId = dashboardId,
        WidgetId = request.WidgetId,
        Row = request.Row,
        RowSpan = request.RowSpan,
        Column = request.Column,
        ColumnSpan = request.ColumnSpan,
    };
}
