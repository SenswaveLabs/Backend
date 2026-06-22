namespace Senswave.Devices.UnitTests.Operations.Types.HexColorOperation;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseHexColorTests
{
    [Fact]
    public async Task Validate()
    {
        // Arrange
        var operation = Operation;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();
        var parsedOperation = implementation.AsOperationEntity();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(parsedOperation.Configuration["isJson"]!.GetValue<bool>());
    }

    [Fact]
    public async Task ValidateJson()
    {
        // Arrange
        var operation = JsonOperation;

        // Act
        var implementation = factory.Create(operation).Data;
        var result = await implementation.Validate();
        var parsedOperation = implementation.AsOperationEntity();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(parsedOperation.Configuration["isJson"]!.GetValue<bool>());
    }
}
