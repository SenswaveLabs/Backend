namespace Senswave.Devices.UnitTests.Widgets.Types.Color;

[Trait("Collection", "UnitTests")]
public class ColorValidationTests : BaseColorTests
{
    [Fact]
    public async Task ValidColor()
    {
        // Arrange
        var widget = ColorWidget;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task InvalidIntegerOperation()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Operation = IntegerOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task InvalidTextOperation()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Operation = TextOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ValidNumberOperation()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Operation = NumberOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ValidBooleanOperation()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Operation = BooleanOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ValidOptionsOperation()
    {
        // Arrange
        var widget = ColorWidget;
        widget.Operation = OptionsOperation;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }
}
