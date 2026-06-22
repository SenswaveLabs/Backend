using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Display;

[Trait("Collection", "UnitTests")]
public class DisplayActionTests : BaseDisplayTests
{
    [Fact]
    public async Task ActionIsDisabled()
    {
        // Arrange
        var widget = NumberRangedWidget;
        var jsonValue = JsonValue.Create(11);
        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var result = createdWidget.Data.PreprocessAction(jsonValue);

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.True(result.IsFailure);
    }
}
