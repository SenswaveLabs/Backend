using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.OptionOperation;

[Trait("Collection", "UnitTests")]
public class CompilancyTests : BaseOptionTest
{
    [Theory]
    [InlineData("Option1")]
    [InlineData("Option2")]
    [InlineData("Option3")]
    [InlineData("Option4")]
    public async Task ValuesAreCompliant(string inlineValue)
    {
        //Arrange
        var operation = Operation;
        var testValue = JsonValue.Create(inlineValue)!;

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = await operatinImplementation.Data.IsValueCompliant(testValue);

        //Assert
        Assert.True(value.IsSuccess);
    }

    [Theory]
    [InlineData("Value")]
    [InlineData(12.31)]
    [InlineData(1)]
    [InlineData(true)]
    [InlineData("Option0")]
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
