using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Radio;

[Trait("Collection", "UnitTests")]
public class RadioActionTests : BaseRadioTests
{
    [Fact]
    public async Task ActionDoesNotTranslateValue()
    {
        // Arrange
        var widget = RadioWidget;
        var jsonValue = JsonValue.Create(11);
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
        var widget = RadioWidget;
        widget.Enabled = false;
        var jsonValue = JsonValue.Create(!false);
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
