namespace Senswave.Devices.UnitTests.Operations.Types.BooleanOperation;

[Trait("Collection", "UnitTests")]
public class AsOperationEntityTests : BaseBooleanTest
{
    [Fact]
    public async Task DisplaysDefaultValues()
    {
        //Arrange
        var operation = Operation;

        //Act
        var operatinImplementation = factory.Create(operation).Data;
        var value = operatinImplementation.AsOperationEntity();

        //Assert
        Assert.Equal(JsonValueKind.False, value.Configuration["falseValue"]!.GetValueKind());
        Assert.Equal(JsonValueKind.True, value.Configuration["trueValue"]!.GetValueKind());
    }

    [Fact]
    public async Task DisplaysMappedValues()
    {
        //Arrange
        var operation = Operation;
        operation.Configuration["trueValue"] = "test";
        operation.Configuration["falseValue"] = 1;

        //Act
        var operatinImplementation = factory.Create(operation).Data;
        var value = operatinImplementation.AsOperationEntity();

        //Assert
        Assert.Equal("1", value.Configuration["falseValue"]!.ToString());
        Assert.Equal("test", value.Configuration["trueValue"]!.ToString());
    }
}