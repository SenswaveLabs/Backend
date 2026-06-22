using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.BooleanOperation;

[Trait("Collection", "UnitTests")]
public class CompliancyTests : BaseBooleanTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ValuesAreCompliant(bool inlineValue)
    {
        //Arrange
        var operation = Operation;
        var testValue = JsonValue.Create(inlineValue);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = await operatinImplementation.Data.IsValueCompliant(testValue);

        //Assert
        Assert.True(value.IsSuccess);
    }

    [Theory]
    [InlineData("false")]
    [InlineData("value")]
    [InlineData(1)]
    public async Task ValuesAreNotCompliant(object inlineValue)
    {
        //Arrange
        var operation = Operation;
        var testValue = JsonValue.Create(inlineValue)!;

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = await operatinImplementation.Data.IsValueCompliant(testValue);

        //Assert
        Assert.False(value.IsSuccess);
    }
}