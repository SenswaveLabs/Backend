using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.TileTypes;
using Senswave.Devices.Domain.Devices.TileTypes.Display;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Display;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseDisplayTileTests
{
    [Fact]
    public async Task Validate_NumberOperation_ReturnsSuccess()
    {
        var tileResult = deviceTileFactory.Create(NumberDisplayDevice());

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Validate();
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Validate_IntegerOperation_ReturnsSuccess()
    {
        var tileResult = deviceTileFactory.Create(IntegerDisplayDevice());

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Validate();
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(OperationType.Boolean)]
    [InlineData(OperationType.Text)]
    [InlineData(OperationType.Options)]
    [InlineData(OperationType.HexColor)]
    [InlineData(OperationType.Invalid)]
    [InlineData(OperationType.Empty)]
    public async Task Validate_NonNumericOperation_ReturnsFailure(OperationType operationType)
    {
        var mockOperation = new Mock<IOperation>();
        mockOperation.Setup(o => o.Type).Returns(operationType);

        var displayTile = new DisplayDeviceTile(
            new DeviceTile { Type = DeviceTileType.Display },
            new DisplayDeviceTileConfiguration(),
            mockOperation.Object,
            new Mock<ILogger<IDeviceTile>>().Object);

        var result = await displayTile.Validate();

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Validate_UnitTooLong_ReturnsFailure()
    {
        var tileResult = deviceTileFactory.Create(NumberDisplayDevice(unit: "meters/s"));

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Validate();
        Assert.True(result.IsFailure);
    }
}
