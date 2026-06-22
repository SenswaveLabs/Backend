using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Enums;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Dashboards.Types.GridDashboard;

[Trait("Collection", "UnitTests")]
public class GridDashboardDisplayTests : BaseDashboardTest
{
    [Fact]
    public async Task GridDashboardDisplayed()
    {
        // Arrange
        var vertical = BooleanWidget(true);
        var verticalWidget = new JsonObject()
        {
            ["widgetId"] = vertical.Id,
            ["row"] = 0,
            ["rowSpan"] = 1,
            ["column"] = 0,
            ["columnSpan"] = 5,
        };

        var horizontal = BooleanWidget(true);
        var horizontalWidget = new JsonObject()
        {
            ["widgetId"] = horizontal.Id,
            ["row"] = 2,
            ["rowSpan"] = 1,
            ["column"] = 0,
            ["columnSpan"] = 5,
        };

        var dashboard = new Dashboard()
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
                ["positionedWidgets"] = new JsonArray([verticalWidget, horizontalWidget])
            }
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        widgetQueryRepository.Setup(x => x.GetWidgetsWithOperation(It.IsAny<List<Guid>>(), default))
            .ReturnsAsync([horizontal, vertical]);

        // Act
        var data = await dashboardFactory.Create(dashboard.Id, default);
        var displayResult = await data.Data.ToDisplay(default);

        // Assert
        Assert.True(data.IsSuccess);
        Assert.True(displayResult.IsSuccess);
        Assert.True(displayResult.Data.Configuration["rows"]!.GetValue<int>() == 5);
        Assert.True(displayResult.Data.Configuration["columns"]!.GetValue<int>() == 5);
        Assert.Equal(2, displayResult.Data.Configuration["positionedWidgets"]!.AsArray().Count);
        Assert.Equal(2, displayResult.Data.Configuration["calculatedWidgets"]!.AsArray().Count);
        Assert.Contains(horizontal.Id, displayResult.Data.Configuration["positionedWidgets"]!.AsArray().Select(x => x!["widgetId"]!.GetValue<Guid>()));
        Assert.Contains(horizontal.Id, displayResult.Data.Configuration["calculatedWidgets"]!.AsArray().Select(x => x!["id"]!.GetValue<Guid>()));
    }
}
