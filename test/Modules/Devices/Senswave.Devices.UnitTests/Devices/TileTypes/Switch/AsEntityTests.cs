namespace Senswave.Devices.UnitTests.Devices.TileTypes.Switch;

[Trait("Collection", "UnitTests")]
public class AsEntityTests : BaseSwitchTileTests
{
    [Fact]
    public void ConfigurationIsAlwaysEmpty()
    {
        var device = BooleanSwitchDevice();

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Empty(entity.Configuration);
    }

    [Fact]
    public void StripsOverflowFields()
    {
        var device = BooleanSwitchDevice();
        device.Tile.Configuration["overflow"] = "extra";

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Empty(entity.Configuration);
    }
}
