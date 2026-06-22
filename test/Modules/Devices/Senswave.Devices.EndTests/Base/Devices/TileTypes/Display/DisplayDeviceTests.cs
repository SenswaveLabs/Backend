using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.TestInfrastructure.TestSetup.Models;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.Base.Devices.TileTypes.Display;

[Trait("Collection", "EndTest")]
public class DisplayDeviceTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task DisplayTileTypeReturnedInResponse()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostNumberOperation(deviceId);
        await PatchDeviceWithDisplayTile(deviceId, operationId);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}/display");
        var responseContent = await response.Content.ReadAsStringAsync();
        var device = JsonNode.Parse(responseContent)!.AsObject();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Display", device["tile"]!["type"]!.GetValue<string>());
        Assert.Null(device["tile"]!["value"]);
    }

    [Fact]
    public async Task DisplayTileReturnsOperationValue()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostNumberOperation(deviceId);
        await PatchDeviceWithDisplayTile(deviceId, operationId);

        using var scope = Factory.Server.Services.CreateScope();
        var devicesContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();

        var queriedOperation = await devicesContext.Operations
            .FirstAsync(x => x.Id == operationId);

        queriedOperation.Values.Add(new OperationValue
        {
            Operation = queriedOperation,
            InternalValue = new() { ["value"] = 21.5 },
            ProcessedAtUtc = DateTime.UtcNow,
        });
        await devicesContext.SaveChangesAsync();

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}/display");
        var responseContent = await response.Content.ReadAsStringAsync();
        var device = JsonNode.Parse(responseContent)!.AsObject();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Display", device["tile"]!["type"]!.GetValue<string>());
        Assert.Equal(21.5, device["tile"]!["value"]!.AsValue().GetValue<double>());
    }

    [Fact]
    public async Task DisplayTileHasConfigurationWithUnit()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostIntegerRangedOperation(deviceId);

        var patchRequest = RequestFactory.CreateAssignOperationToDevice(
            displayableOperationId: operationId.ToString(),
            type: "Display");

        await AuthorizeClientAsUser(client);
        await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", patchRequest.Serialize());

        using var scope = Factory.Server.Services.CreateScope();
        var devicesContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var tile = await devicesContext.DeviceTiles.FirstAsync(x => x.DeviceId == deviceId);
        tile.Configuration = new JsonObject { ["unit"] = "°C" };
        await devicesContext.SaveChangesAsync();

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}/display");
        var responseContent = await response.Content.ReadAsStringAsync();
        var device = JsonNode.Parse(responseContent)!.AsObject();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tileNode = device["tile"]!.AsObject();
        Assert.NotNull(tileNode["configuration"]);
        Assert.Equal("°C", tileNode["configuration"]!.AsObject()["unit"]!.ToString().Trim());
    }
}
