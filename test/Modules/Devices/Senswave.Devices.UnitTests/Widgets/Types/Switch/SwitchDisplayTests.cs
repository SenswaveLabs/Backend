namespace Senswave.Devices.UnitTests.Widgets.Types.Switch;

[Trait("Collection", "UnitTests")]
public class SwitchDisplayTests : BaseSwitchTests
{
    [Fact]
    public async Task DisplayWidget()
    {
        // Arrange
        var widget = BooleanWidget;
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
        Assert.Equal(latestValue.Value.ToString(), display.Configuration["runtime"]!["value"]!.ToString());
    }

    [Fact]
    public async Task DisplayWidgetWithoutValue()
    {
        // Arrange
        var widget = BooleanWidget;
        widget.Operation!.Values = [];
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(createdWidget.Data);
        Assert.False(display.Configuration.ContainsKey("runtime"));
    }
}
