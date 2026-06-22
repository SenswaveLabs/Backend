namespace Senswave.Devices.UnitTests.Devices.TileTypes.Default;

[Trait("Collection", "UnitTests")]
public class AsEntityTests : BaseDefaultTileTests
{
    [Fact]
    public void ReturnsUnchangedTile()
    {
        var device = DefaultTileDevice();

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Same(device.Tile, entity);
    }

    [Fact]
    public void DoesNotStripUnknownFields()
    {
        var device = DefaultTileDevice();
        device.Tile.Configuration["extra"] = "value";

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Equal("value", entity.Configuration["extra"]!.GetValue<string>());
    }
}
