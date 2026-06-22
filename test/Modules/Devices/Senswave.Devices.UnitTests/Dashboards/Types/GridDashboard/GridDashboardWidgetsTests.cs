using Senswave.Devices.Domain.Dashboards.Types.Gird;

namespace Senswave.Devices.UnitTests.Dashboards.Types.GridDashboard;

[Trait("Collection", "UnitTests")]
public class GridDashboardWidgetsTests : BaseDashboardTest
{
    [Fact]
    public async Task WidgetAddedToDashboard()
    {
        // Arrange
        var dashboard = GridDashboard;
        var positionedWidget = new PositionedWidget
        {
            WidgetId = Guid.NewGuid(),
            ColumnSpan = 1,
            Column = 0,
            Row = 1,
            RowSpan = 3,
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        // Act
        var dashboardResult = await dashboardFactory.Create(dashboard.Id, default);
        var addingResult = dashboardResult.Data.AddWidget(positionedWidget);

        // Assert
        Assert.True(dashboardResult.IsSuccess);
        Assert.True(addingResult.IsSuccess);
    }

    [Fact]
    public async Task AddedWidgetIsInList()
    {
        // Arrange
        var dashboard = GridDashboard;
        var positionedWidget = new PositionedWidget
        {
            WidgetId = Guid.NewGuid(),
            ColumnSpan = 1,
            Column = 0,
            Row = 1,
            RowSpan = 3,
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        // Act
        var dashboardResult = await dashboardFactory.Create(dashboard.Id, default);
        var addingResult = dashboardResult.Data.AddWidget(positionedWidget);
        var outputDashboard = dashboardResult.Data.ToDashboard();

        // Assert
        Assert.True(dashboardResult.IsSuccess);
        Assert.True(addingResult.IsSuccess);
        Assert.Equal(2, outputDashboard.Configuration["positionedWidgets"]!.AsArray().Count);
    }

    [Fact]
    public async Task CannotDuplicateWidget()
    {
        // Arrange
        var dashboard = GridDashboard;
        var widgetId = Guid.NewGuid();

        var positionedWidget = new PositionedWidget
        {
            WidgetId = widgetId,
            ColumnSpan = 1,
            Column = 0,
            Row = 1,
            RowSpan = 3,
        };

        var duplicatePositionedWidget = new PositionedWidget
        {
            WidgetId = widgetId,
            ColumnSpan = 1,
            Column = 0,
            Row = 2,
            RowSpan = 3,
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        // Act
        var dashboardResult = await dashboardFactory.Create(dashboard.Id, default);
        var addingResult = dashboardResult.Data.AddWidget(positionedWidget);
        var duplicateResult = dashboardResult.Data.AddWidget(duplicatePositionedWidget);

        // Assert
        Assert.True(dashboardResult.IsSuccess);
        Assert.True(addingResult.IsSuccess);
        Assert.False(duplicateResult.IsSuccess);
    }

    [Fact]
    public async Task FailsToPlaceWidgetOnWidget()
    {
        // Arrange
        var dashboard = GridDashboard;
        var positionedWidget = new PositionedWidget
        {
            WidgetId = Guid.NewGuid(),
            Row = 0,
            RowSpan = 1,
            ColumnSpan = 3,
            Column = 0,
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        // Act
        var dashboardResult = await dashboardFactory.Create(dashboard.Id, default);
        var addingResult = dashboardResult.Data.AddWidget(positionedWidget);

        // Assert
        Assert.True(dashboardResult.IsSuccess);
        Assert.True(addingResult.IsFailure);
    }

    [Fact]
    public async Task RowOutOfBounds()
    {
        // Arrange
        var dashboard = GridDashboard;
        var positionedWidget = new PositionedWidget
        {
            WidgetId = Guid.NewGuid(),
            ColumnSpan = 1,
            Column = 0,
            Row = 1,
            RowSpan = 5,
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        // Act
        var dashboardResult = await dashboardFactory.Create(dashboard.Id, default);
        var addingResult = dashboardResult.Data.AddWidget(positionedWidget);

        // Assert
        Assert.True(dashboardResult.IsSuccess);
        Assert.True(addingResult.IsFailure);
    }

    [Fact]
    public async Task ColumnsOutOfBounds()
    {
        // Arrange
        var dashboard = GridDashboard;
        var positionedWidget = new PositionedWidget
        {
            WidgetId = Guid.NewGuid(),
            ColumnSpan = 6,
            Column = 0,
            Row = 1,
            RowSpan = 3,
        };

        dashboardQueryRepository.Setup(x => x.GetDashboard(dashboard.Id, default))
            .ReturnsAsync(dashboard);

        // Act
        var dashboardResult = await dashboardFactory.Create(dashboard.Id, default);
        var addingResult = dashboardResult.Data.AddWidget(positionedWidget);

        // Assert
        Assert.True(dashboardResult.IsSuccess);
        Assert.True(addingResult.IsFailure);
    }
}
