using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.IntegerOperation;

[Trait("Collection", "UnitTests")]
public class CompliancyTests : BaseIntegerTest
{
    [Theory]
    [InlineData(1234)]
    [InlineData(-13245)]
    public async Task ValuesAreCompliant(int inlineValue)
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
    [InlineData(13245.234)]
    [InlineData(-13245.4234)]
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
    [InlineData(1234)]
    [InlineData(-13245)]
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
