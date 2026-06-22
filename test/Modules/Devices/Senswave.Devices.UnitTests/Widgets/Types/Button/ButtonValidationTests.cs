namespace Senswave.Devices.UnitTests.Widgets.Types.Button;

[Trait("Collection", "UnitTests")]
public class ButtonValidationTests : BaseButtonTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ValidBooleanButton(bool value)
    {
        // Arrange
        var widget = BooleanWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    [InlineData(0)]
    [InlineData(123.1)]
    public async Task InvalidBooleanButton(object value)
    {
        // Arrange
        var widget = BooleanWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(-100)]
    public async Task ValidIntegerButton(int value)
    {
        // Arrange
        var widget = IntegerRangedWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(-101)]
    [InlineData(true)]
    [InlineData("true")]
    public async Task InvalidIntegerButton(object value)
    {
        // Arrange
        var widget = IntegerRangedWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(99.9)]
    [InlineData(-99.9)]
    public async Task ValidNumberButton(int value)
    {
        // Arrange
        var widget = NumberRanngedWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(100.1)]
    [InlineData(-100.1)]
    [InlineData(true)]
    [InlineData("true")]
    public async Task InvalidNumberButton(object value)
    {
        // Arrange
        var widget = NumberRanngedWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("Test spaces")]
    public async Task ValidTextWidget(string text)
    {
        // Arrange
        var widget = TextWidget(text);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(100.1)]
    [InlineData(-100.1)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InvalidTextWidget(object value)
    {
        // Arrange
        var widget = TextWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task ValidOptionWidet()
    {
        // Arrange
        var widget = OptionWidget("Option1");

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("Option34")]
    [InlineData("1")]
    [InlineData("test")]
    public async Task InvalidOptionWidget(string value)
    {
        // Arrange
        var widget = OptionWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("#FFFFFF")]
    [InlineData("#000")]
    public async Task ValidHexColorWidget(string color)
    {
        // Arrange
        var widget = HexColorWidget(color);
        // Act
        var result = await BaseValidationTest(widget);
        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("#GGGGGG")]
    [InlineData("#12345")]
    [InlineData("#1234567")]
    [InlineData(true)]
    [InlineData(231.123)]
    public async Task InvalidHexColorWidget(object value)
    {
        // Arrange
        var widget = HexColorWidget(value);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task TooLongWidgetOption()
    {
        // Arrange
        var tooLongText = new string('a', 1025); // Assuming max length is 255
        var widget = TextWidget(tooLongText);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }
}
