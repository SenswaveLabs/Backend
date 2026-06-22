

using Senswave.Devices.Domain.Operations.Types.Characteristics.Range;

namespace Senswave.Devices.UnitTests.Operations.Types.NumberOperation;

[Trait("Collection", "UnitTests")]
public class RangeCharacteristicTests : BaseNumberTest
{
    [Fact]
    public async Task CorrectRangeReturned()
    {
        //Arrange
        var operationEntity = RangedOperation;

        //Act
        var operatinImplementation = factory.Create(operationEntity);
        var operation = operatinImplementation.Data as IRangeCharacteristic;
        var result = await operation!.GetDisplayRange();

        //Assert
        Assert.Equal(operationEntity.Configuration["min"]!.ToString(), result.Data["min"]!.ToString());
        Assert.Equal(operationEntity.Configuration["max"]!.ToString(), result.Data["max"]!.ToString());
    }

    [Theory]
    [InlineData(0.001)]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(20.02)]
    public async Task ValidStepValue(double step)
    {
        //Arrange
        var operationEntity = RangedOperation;

        //Act
        var operatinImplementation = factory.Create(operationEntity);
        var operation = operatinImplementation.Data as IRangeCharacteristic;
        var result = await operation!.ValidateStep(step);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0.0001)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-0.01)]
    public async Task InvalidStepValue(double step)
    {
        //Arrange
        var operationEntity = RangedOperation;

        //Act
        var operatinImplementation = factory.Create(operationEntity);
        var operation = operatinImplementation.Data as IRangeCharacteristic;
        var result = await operation!.ValidateStep(step);

        //Assert
        Assert.False(result.IsSuccess);
    }
}
