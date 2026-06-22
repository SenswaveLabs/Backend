using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;

namespace Senswave.Devices.UnitTests.Widgets.Types;

[Trait("Collection", "UnitTests")]
public class BaseValidationTests : BaseWidgetTests
{
    [Fact]
    public async Task TooLongNameForWidget()
    {
        // Arrange
        var operation = BooleanOperation;
        var widget = new Widget
        {
            Id = Guid.NewGuid(),

            Operation = operation,
            OperationId = operation.Id,

            Enabled = true,
            Name = new string('a', 65),
            Configuration = new()
            {
                ["value"]=true
            },
            Type = WidgetType.Button,
        };

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);

    }

    [Fact]
    public async Task TooShortName()
    {
        // Arrange
        var operation = BooleanOperation;
        var widget = new Widget
        {
            Id = Guid.NewGuid(),

            Operation = operation,
            OperationId = operation.Id,

            Enabled = true,
            Name = new string('a', 2),
            Configuration = new()
            {
                ["value"]=true
            },
            Type = WidgetType.Button,
        };

        // Act
        var result = await BaseValidationTest(widget);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task OperationCannotBeEmpty()
    {
        // Arrange
        var operation = BooleanOperation;
        var widget = new Widget
        {
            Id = Guid.NewGuid(),

            Enabled = true,
            Name = new string('a', 65),
            Configuration = new()
            {
                ["value"]=true
            },
            Type = WidgetType.Button,
        };

        // Act
        var result = await widgetFactory.Create(widget.Id, default);

        // Assert
        Assert.True(result.IsFailure);
    }
}
