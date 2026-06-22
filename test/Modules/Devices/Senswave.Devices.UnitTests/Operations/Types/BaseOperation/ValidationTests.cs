using Senswave.Devices.Domain.Operations.Entities;
using Senswave.Devices.Domain.Operations.Enums;

namespace Senswave.Devices.UnitTests.Operations.Types.BaseOperation;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseOperationTest
{
    [Fact]
    public async Task ValidateName()
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
            Name = new string('a', 129),
            Type = OperationType.Boolean,
        };

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();

        // Assert
        Assert.True(result.IsFailure);
    }
}
