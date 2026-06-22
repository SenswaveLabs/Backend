using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace Senswave.Devices.EndTests.Base.Devices.TileTypes.Switch;

[Trait("Collection", "EndTest")]
public class DisplayDevicesTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task SwitchTileHasValue()
    {

    }
}
