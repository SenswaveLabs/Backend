namespace Senswave.Devices.UnitTests.Devices.TileTypes.Default;

[Trait("Collection", "UnitTests")]
public class ValidationTests : BaseDefaultTileTests
{
    [Fact]
    public async Task Validate_ReturnsSuccess()
    {
        var tileResult = deviceTileFactory.Create(DefaultTileDevice());

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Validate();
        Assert.True(result.IsSuccess);
    }
}
