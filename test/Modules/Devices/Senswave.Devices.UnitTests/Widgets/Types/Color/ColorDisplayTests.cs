namespace Senswave.Devices.UnitTests.Widgets.Types.Color;

[Trait("Collection", "UnitTests")]
public class ColorDisplayTests : BaseColorTests
{
    [Fact]
    public async Task DisplayWidget()
    {
        // Arrange
        var widget = ColorWidget;
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
        Assert.True(display.Configuration.ContainsKey("runtime"));
        Assert.NotNull(latestValue.Value.ToString());
        Assert.Equal(latestValue.Value.ToString(), display.Configuration["runtime"]!["value"]!.ToString());
    }

    [Fact]
    public async Task DisplayWidgetWithoutValue()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Operation!.Values = []; // No values

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
