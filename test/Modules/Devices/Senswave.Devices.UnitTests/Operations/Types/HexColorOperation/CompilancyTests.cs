
using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.HexColorOperation;

[Trait("Collection", "UnitTests")]
public class CompilancyTests : BaseHexColorTests
{
    [Theory]
    [InlineData("#FFFFFF")]
    [InlineData("#000000")]
    [InlineData("#123")]
    public async Task ValuesAreCompliant(string inlineValue)
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
    [InlineData("#ffff")]
    [InlineData("#ABCDEG")]
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
}
