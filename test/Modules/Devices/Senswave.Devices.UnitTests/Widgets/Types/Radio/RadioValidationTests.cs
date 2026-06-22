using Senswave.Devices.Domain.Widgets.Types.Radio.Model;
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Widgets.Types.Radio;

[Trait("Collection", "UnitTests")]
public class RadioValidationTests : BaseRadioTests
{
    [Fact]
    public async Task OptionsRequired()
    {
        // Arrange
        var widget = RadioWidget;
        widget.Configuration["options"] = JsonValue.Create(Array.Empty<RadioOption>());

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(11)]
    public async Task InvalidOptionCount(int count)
    {
        // Arrange
        var widget = RadioWidget;
        var options = Enumerable.Range(0, count)
            .Select(i => new RadioOption
            {
                DisplayName = $"Option{i}",
                OptionName = $"Opt{i}",
                Icon = $"Icon{i}"
            }).ToList();

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ValidWidget()
    {
        // Arrange
        var widget = RadioWidget;

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("Invalid!")]
    [InlineData("TooLongDisplayNameHere")]
    public async Task InvalidDisplayName(string displayName)
    {
        // Arrange
        var widget = RadioWidget;
        var options = new List<RadioOption>
        {
            new() { DisplayName = displayName, OptionName = "Option1", Icon = "Icon1" },
            new() { DisplayName = "ValidName", OptionName = "Option2", Icon = "Icon2" }
        };

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("Name123")]
    public async Task ValidDisplayName(string displayName)
    {
        // Arrange
        var widget = RadioWidget;
        var options = new List<RadioOption>
        {
            new() { DisplayName = displayName, OptionName = "Option1", Icon = "Icon1" },
            new() { DisplayName = "Another", OptionName = "Option2", Icon = "Icon2" }
        };

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DuplicateDisplayNamesAreInvalid()
    {
        // Arrange
        var widget = RadioWidget;
        var options = new List<RadioOption>
        {
            new() { DisplayName = "Same", OptionName = "Option1", Icon = "Icon1" },
            new() { DisplayName = "Same", OptionName = "Option2", Icon = "Icon2" }
        };

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task DuplicateOptionNamesAreInvalid()
    {
        // Arrange
        var widget = RadioWidget;
        var options = new List<RadioOption>
        {
            new() { DisplayName = "First", OptionName = "Option1", Icon = "Icon1" },
            new() { DisplayName = "Second", OptionName = "Option1", Icon = "Icon2" }
        };

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData("Invalid!Icon")]
    [InlineData("ThisIconNameIsWayTooLongToBeAccepted123ThisIconNameIsWayTooLongToBeAccepted123ThisIconNameIsWayTooLongToBeAccepted123ThisIconNameIsWayTooLongToBeAccepted123")]
    public async Task InvalidIcon(string icon)
    {
        // Arrange
        var widget = RadioWidget;
        var options = new List<RadioOption>
        {
            new() { DisplayName = "First", OptionName = "Option1", Icon = icon },
            new() { DisplayName = "Second", OptionName = "Option2", Icon = "Icon2" }
        };

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task OptionNameNotInOperationOptionsIsInvalid()
    {
        // Arrange
        var widget = RadioWidget;
        var options = new List<RadioOption>
        {
            new() { DisplayName = "Option 1", OptionName = "Option1", Icon = "Icon1" },
            new() { DisplayName = "Option 2", OptionName = "NonExistentOption", Icon = "Icon2" }
        };

        widget.Configuration["options"] = JsonSerializer.SerializeToNode(options);

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.False(result.IsSuccess);
    }
}
