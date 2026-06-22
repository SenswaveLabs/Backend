using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.UnitTests.Operations.Types;

namespace Senswave.Devices.UnitTests.Operations.Factory;

[Trait("Collection", "UnitTests")]
public class OperationFactoryTests : BaseOperationTest
{
    [Fact]
    public void ValidateEmptyType()
    {
        // Arrange
        var operation = new Operation()
        {
            DataReferenceId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),

            Id = Guid.NewGuid(),
            Configuration = new()
            {
                ["isJson"] = false,
            },
            Name = "TestOperation",
            Type = OperationType.Empty,
        };

        // Act
        var implementation = factory.Create(operation);

        // Assert
        Assert.False(implementation.IsSuccess);
    }

    [Fact]
    public void ValidateInvalidType()
    {
        // Arrange
        var operation = new Operation()
        {
            DataReferenceId = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),

            Id = Guid.NewGuid(),
            Configuration = new()
            {
                ["isJson"] = false,
            },
            Name = "TestOperation",
            Type = OperationType.Invalid,
        };

        // Act
        var implementation = factory.Create(operation);

        // Assert
        Assert.False(implementation.IsSuccess);
    }
}
