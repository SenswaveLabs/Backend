namespace Senswave.Devices.UnitTests.Widgets.Types.Slider;

[Trait("Collection", "UnitTests")]
public class SliderDisplayTests : BaseSliderTests
{
    [Fact]
    public async Task DisplayWidget()
    {
        // Arrange
        var widget = NumberRangedWidget;
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        var latestValue = widget.Operation!.Values
            .OrderBy(x => x.ProcessedAtUtc)
            .Last();

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(createdWidget.Data);
        Assert.Equal(widget.Configuration["step"]!.ToString(), display.Configuration["step"]!.ToString());
        Assert.Equal(widget.Operation.Configuration["min"]!.ToString(), display.Configuration["range"]!["min"]!.ToString());
        Assert.Equal(widget.Operation.Configuration["max"]!.ToString(), display.Configuration["range"]!["max"]!.ToString());
        Assert.Equal(latestValue.Value.ToString(), display.Configuration["runtime"]!["value"]!.ToString());
    }

    [Fact]
    public async Task DisplayWidgetWithoutValue()
    {
        // Arrange
        var widget = NumberRangedWidget;
        widget.Operation!.Values = [];
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(createdWidget.Data);
        Assert.Equal(widget.Configuration["step"]!.ToString(), display.Configuration["step"]!.ToString());
        Assert.Equal(widget.Operation.Configuration["min"]!.ToString(), display.Configuration["range"]!["min"]!.ToString());
        Assert.Equal(widget.Operation.Configuration["max"]!.ToString(), display.Configuration["range"]!["max"]!.ToString());
        Assert.False(display.Configuration.ContainsKey("runtime"));
    }
}
