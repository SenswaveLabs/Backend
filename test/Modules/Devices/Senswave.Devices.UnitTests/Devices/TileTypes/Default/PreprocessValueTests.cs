using System.Text.Json.Nodes;

namespace Senswave.Devices.UnitTests.Devices.TileTypes.Default;

[Trait("Collection", "UnitTests")]
public class PreprocessValueTests : BaseDefaultTileTests
{
    [Fact]
    public async Task Preprocess_ReturnsFailure()
    {
        var tileResult = deviceTileFactory.Create(DefaultTileDevice());

        Assert.True(tileResult.IsSuccess);
        var result = await tileResult.Data.Preprocess(JsonValue.Create("test")!);
        Assert.True(result.IsFailure);
    }
}
