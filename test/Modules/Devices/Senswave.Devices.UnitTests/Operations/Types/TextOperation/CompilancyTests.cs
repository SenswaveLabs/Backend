using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Operations.Types.TextOperation;

[Trait("Collection", "UnitTests")]
public class CompilancyTests : BaseTextTest
{
    [Theory]
    [InlineData("sdfgsdfg3q45")]
    [InlineData("#FFFFFF")]
    [InlineData("+-_123SDFsdf()")]
    public async Task ValuesAreCompliant(string values)
    {
        //Arrange
        var operation = Operation;
        var testValue = JsonValue.Create(values);

        //Act
        var operatinImplementation = factory.Create(operation);
        var value = await operatinImplementation.Data.IsValueCompliant(testValue);

        //Assert
        Assert.True(value.IsSuccess);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    [InlineData(13245)]
    [InlineData(-13245)]
    [InlineData(-13245.08910)]
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
