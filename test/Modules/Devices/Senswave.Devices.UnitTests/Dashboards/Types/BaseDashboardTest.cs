using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Enums;
using Senswave.Devices.Domain.Dashboards.Factories;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using Senswave.Devices.UnitTests.Widgets;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Dashboards.Types;

public abstract class BaseDashboardTest : BaseWidgetTests
{
    protected readonly DashboardFactory dashboardFactory;

    protected readonly Mock<IDashboardsQueryRespository> dashboardQueryRepository;

    protected BaseDashboardTest()
    {
        dashboardQueryRepository = new();
        var logger = new Mock<ILogger<DashboardFactory>>();
        dashboardFactory = new DashboardFactory(dashboardQueryRepository.Object, widgetFactory, logger.Object);
    }

    protected Widget BooleanWidget(object value) => new()
    {
        Id = Guid.NewGuid(),
        Name = "TestWidget",
        Type = WidgetType.Button,
        Enabled = true,
        Configuration = new()
        {
            ["value"] = JsonValue.Create(value)
        },
        Operation = BooleanOperation,
        OperationId = Guid.NewGuid(),
    };

    protected Dashboard GridDashboard => new()
    {
        DeviceId = Guid.NewGuid(),
        Device = new(),

        Icon = "psp",
        Name = "Name",
        Type = DashboardType.Grid,
        Configuration = new()
        {
            ["rows"] = 5,
            ["columns"] = 5,
            ["positionedWidgets"] = new JsonArray(new JsonObject
            {
                ["widgetId"] = Guid.NewGuid(),
                ["row"] = 0,
                ["rowSpan"] = 1,
                ["column"] = 0,
                ["columnSpan"] = 2,
            })
        }
    };
}
