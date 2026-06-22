
namespace Senswave.Devices.UnitTests.Widgets.Types.Radio;

[Trait("Collection", "UnitTests")]
public class RadioDisplayTests : BaseRadioTests
{
    [Fact]
    public async Task DisplayWidget()
    {
        // Arrange
        var widget = RadioWidget;
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        var latestValue = widget.Operation!.Values
            .OrderBy(x => x.ProcessedAtUtc)
            .Last();


        var expectedOptionsCount = widget.Configuration["options"]!.AsArray().Count;

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(createdWidget.Data);
        Assert.Equal(expectedOptionsCount, display.Configuration["options"]!.AsArray().Count);
        Assert.True(display.Configuration.ContainsKey("runtime"));
        Assert.Equal(latestValue.Value.ToString(), display.Configuration["runtime"]!["value"]!.ToString());
    }

    [Fact]
    public async Task DisplayWidgetWithoutValue()
    {
        // Arrange
        var widget = RadioWidget;
        widget.Operation!.Values = []; // No values

        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        var expectedOptionsCount = widget.Configuration["options"]!.AsArray().Count;

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(createdWidget.Data);

        Assert.Equal(expectedOptionsCount, display.Configuration["options"]!.AsArray().Count);

        Assert.False(display.Configuration.ContainsKey("runtime"));
    }
}
