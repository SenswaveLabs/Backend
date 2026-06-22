using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.TileTypes;
using Senswave.Devices.Domain.Devices.TileTypes.Switch;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Switch;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseSwitchTileTests
{
    [Fact]
    public async Task Validate_BooleanOperation_ReturnsSuccess()
    {
        var tileResult = deviceTileFactory.Create(BooleanSwitchDevice());

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Validate();
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Validate_NonBooleanOperation_ReturnsFailure()
    {
        var mockOperation = new Mock<IOperation>();
        mockOperation.Setup(o => o.Type).Returns(OperationType.Invalid);

        var switchTile = new SwitchDeviceTile(
            new DeviceTile { Type = DeviceTileType.Switch },
            new SwitchDeviceTileConfiguration(),
            mockOperation.Object,
            new Mock<ILogger<IDeviceTile>>().Object);

        var result = await switchTile.Validate();

        Assert.True(result.IsFailure);
    }
}
