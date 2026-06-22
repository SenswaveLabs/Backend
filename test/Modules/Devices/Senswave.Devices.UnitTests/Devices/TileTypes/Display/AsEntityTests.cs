namespace Senswave.Devices.UnitTests.Devices.TileTypes.Display;

[Trait("Collection", "UnitTests")]
public class AsEntityTests : BaseDisplayTileTests
{
    [Fact]
    public void PreservesKnownFields()
    {
        var device = NumberDisplayDevice(unit: "km/h");

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Equal("km/h", entity.Configuration["unit"]!.GetValue<string>());
    }

    [Fact]
    public void StripsOverflowFields()
    {
        var device = NumberDisplayDevice(unit: "°C");
        device.Tile.Configuration["overflow"] = "extra";

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Equal("°C", entity.Configuration["unit"]!.GetValue<string>());
        Assert.Null(entity.Configuration["overflow"]);
    }

    [Fact]
    public void EmptyConfigSerializesDefaults()
    {
        var device = NumberDisplayDevice();

        var tileResult = deviceTileFactory.Create(device);

        Assert.True(tileResult.IsSuccess);
        var entity = tileResult.Data.AsEntity();
        Assert.Equal(string.Empty, entity.Configuration["unit"]!.GetValue<string>());
    }
}
