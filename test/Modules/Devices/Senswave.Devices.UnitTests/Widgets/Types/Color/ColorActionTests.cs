using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Color;

[Trait("Collection", "UnitTests")]
public class ColorActionTests : BaseColorTests
{
    [Fact]
    public async Task ActionDoesNotTranslateValue()
    {
        // Arrange
        var widget = ColorWidget;
        var jsonValue = JsonValue.Create("#FFASDS");
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = createdWidget.Data.PreprocessAction(jsonValue);

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.True(result.IsSuccess);
        Assert.Equal(jsonValue.ToString(), result.Data.ToString());
    }

    [Fact]
    public async Task EnabledFlagWorks()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Enabled = false;
        var jsonValue = JsonValue.Create("#FFASDS");
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = createdWidget.Data.PreprocessAction(jsonValue);

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.False(result.IsSuccess);
    }
}
