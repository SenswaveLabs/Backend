using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Display;

[Trait("Collection", "UnitTests")]
public class PreprocessValueTests : BaseDisplayTileTests
{
    [Fact]
    public async Task Preprocess_ReturnsFailure()
    {
        var tileResult = deviceTileFactory.Create(NumberDisplayDevice());

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Preprocess(JsonValue.Create(42.0)!);
        Assert.True(result.IsFailure);
    }
}
