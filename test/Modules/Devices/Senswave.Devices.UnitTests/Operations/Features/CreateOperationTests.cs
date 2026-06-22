using Senswave.Devices.Application.Operations.Features.CreateOperation;
using Senswave.Devices.Domain.Operations.Enums;

namespace Senswave.Devices.UnitTests.Operations.Features;

[Trait("Collection", "UnitTests")]
public class CreateOperationValidationTests
{
    private readonly CreateOperationValidator validator = new();

    [Fact]
    public void ValidOperation()
    {
        // Arrange
        var operation = new CreateOperationCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Operation",
            Topic = "test",
            Type = OperationType.Boolean,
            Configuration = []
        };

        // Act
        var result = validator.Validate(operation);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void InvalidOperationType()
    {
        // Arrange
        var operation = new CreateOperationCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Operation",
            Topic = "test",
            Type = OperationType.Invalid,
            Configuration = []
        };

        // Act
        var result = validator.Validate(operation);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void EmptyOperationTypeIsNotValid()
    {
        // Arrange
        var operation = new CreateOperationCommand
        {
            DeviceId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Operation",
            Topic = "test",
            Type = OperationType.Empty,
            Configuration = []
        };

        // Act
        var result = validator.Validate(operation);

        // Assert
        Assert.False(result.IsValid);
    }
}

