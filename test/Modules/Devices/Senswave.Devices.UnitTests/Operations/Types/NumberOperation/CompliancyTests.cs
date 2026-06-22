using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.NumberOperation;

[Trait("Collection", "UnitTests")]
public class CompliancyTests : BaseNumberTest
{
    [Theory]
    [InlineData(1234.234)]
    [InlineData(-13245.234)]
    public async Task ValuesAreCompliant(double inlineValue)
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
    [InlineData(13245)]
    [InlineData(-13245)]
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

    [Theory]
    [InlineData(1234.0)]
    [InlineData(-13245.0)]
    public async Task RangedValuesAreCompliant(int inlineValue)
    {
        //Arrange
        var operation = RangedOperation;
        var testValue = JsonValue.Create(inlineValue);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = await operatinImplementation.Data.IsValueCompliant(testValue);

        //Assert
        Assert.False(value.IsSuccess);
    }
}
