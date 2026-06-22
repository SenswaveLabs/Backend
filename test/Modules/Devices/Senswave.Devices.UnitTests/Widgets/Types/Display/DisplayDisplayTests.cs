namespace Senswave.Devices.UnitTests.Widgets.Types.Display;

public class DisplayDisplayTests : BaseDisplayTests
{
    [Fact]
    public async Task LatestValueReturned()
    {
        // Arrange
        var widget = NumberRangedWidget;
        var newestValue = NumberRangedWidget.Operation!.Values
            .OrderByDescending(x => x.ProcessedAtUtc)
            .First();

        widgetQueryRepository.Setup(x => x.GetWidgetWithOperation(widget.Id, default))
            .ReturnsAsync(widget);

        // Act
        var createdWidget = await widgetFactory.Create(widget.Id, default);
        var display = await createdWidget.Data.ToDisplay();

        // Assert
        Assert.True(createdWidget.IsSuccess);
        Assert.NotNull(display);
        Assert.Equal(newestValue.Value, display.Configuration["runtime"]!["value"]);
    }
}
