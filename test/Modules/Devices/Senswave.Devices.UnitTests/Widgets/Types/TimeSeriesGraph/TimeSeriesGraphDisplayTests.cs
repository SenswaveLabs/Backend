namespace Senswave.Devices.UnitTests.Widgets.Types.TimeSeriesGraph;

[Trait("Collection", "UnitTests")]
public class TimeSeriesGraphDisplayTests : BaseTimeSeriesGraphTests
{
    [Fact]
    public async Task DisplayContainsConfiguration()
    {
        // Arrange
        var widget = IntegerWidget;
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(display.Configuration["displayUnit"]);
        Assert.NotNull(display.Configuration["initialNumberOfData"]);
        Assert.NotNull(display.Configuration["displayType"]);
        Assert.NotEmpty(display.Configuration["runtime"]!["values"]!.AsArray());
    }

    [Fact]
    public async Task DisplayContainsRuntimeValues()
    {
        // Arrange
        var widget = IntegerWidget;
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(display.Configuration["runtime"]);
        Assert.NotNull(display.Configuration["runtime"]!["values"]);
    }

    [Fact]
    public async Task RuntimeValuesNotExceedinitialNumberOfData()
    {
        // Arrange
        var widget = IntegerWidget;
        widget.Configuration["initialNumberOfData"] = 2;
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        var values = display.Configuration["runtime"]!["values"]!.AsArray();
        Assert.True(values.Count <= 2);
    }

    [Fact]
    public async Task DisplayTypeReflectedInConfiguration()
    {
        // Arrange
        var widget = IntegerWidget;
        widget.Configuration["displayType"] = "bars";
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.Equal("bars", display.Configuration["displayType"]!.GetValue<string>());
    }
}
