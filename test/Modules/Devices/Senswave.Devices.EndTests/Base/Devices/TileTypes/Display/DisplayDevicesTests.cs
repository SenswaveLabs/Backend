using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace Senswave.Devices.EndTests.Base.Devices.TileTypes.Display;

[Trait("Collection", "EndTest")]
public class DisplayDevicesTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task DisplayTileHasValue()
    {
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operation = await PostNumberOperation(deviceId);
        await PatchDeviceWithDisplayTile(deviceId, operation);

        using var scope = Factory.Server.Services.CreateScope();
        var devicesContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();

        var queriedOperation = await devicesContext.Operations
            .FirstAsync(x => x.Id == operation);

        var value = new OperationValue
        {
            Operation = queriedOperation,
            InternalValue = new()
            {
                ["value"] = 42.5
            },
            ProcessedAtUtc = DateTime.UtcNow,
        };

        queriedOperation.Values.Add(value);
        await devicesContext.SaveChangesAsync();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.GetAsync($"{Paths.DevicesPath}/display?homeId={home}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var devices = JsonNode.Parse(responseContent)!["items"]!.AsArray();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(devices);
        Assert.Equal(42.5, devices[0]!["tile"]!["value"]!.AsValue().GetValue<double>());
    }
}
