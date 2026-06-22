using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Extensions;
using Senswave.Devices.Domain.Operations.ValueObjects;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Devices;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.Base.Devices.TileTypes.Switch;

[Trait("Collection", "EndTest")]
public class DeviceDisplayTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task SwitchTileTypeReturnedInResponse()
    {
        // Arrange
        var client = CreateUnauthorizedClient();

        var home = await PostHome();
        var broker = await PostBroker();
        await PutBrokerForHome(broker, home);
        var room = await PostRoom(home);
        var deviceId = await PostDevice(home, room);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithSwitchTile(deviceId, operationId);
        await AuthorizeClientAsUser(client);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var deviceResponse = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(data);
        Assert.Equal(deviceId, deviceResponse.Id);
        Assert.Equal(room, deviceResponse.RoomId);
        Assert.Equal(operationId.ToString(), deviceResponse.Tile.OperationId);
        Assert.Equal(DeviceTileType.Switch, deviceResponse.Tile.Type.ToDeviceTileType());
    }

    [Fact]
    public async Task SwitchTileValueUpdatedInDisplayResponse()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);

        var request = RequestFactory.CreateAssignOperationToDevice(
            operationId: operationId.ToString(),
            type: "Switch").Serialize();

        using var scope = Factory.Server.Services.CreateScope();
        var devicesContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();

        var tileResponse = await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", request);

        var queriedOperation = await devicesContext.Operations.FirstAsync(x => x.Id == operationId);

        queriedOperation.Values.Add(new OperationValue
        {
            Operation = queriedOperation,
            InternalValue = new() { ["value"] = true },
            ProcessedAtUtc = DateTime.UtcNow,
        });
        await devicesContext.SaveChangesAsync();

        await AuthorizeClientAsUser(client);
        var response = await client.GetAsync($"{Paths.DevicesPath}/display?homeId={home}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var devices = JsonNode.Parse(responseContent)!["items"]!.AsArray();

        queriedOperation.Values.Add(new OperationValue
        {
            Operation = queriedOperation,
            InternalValue = new() { ["value"] = false },
            ProcessedAtUtc = DateTime.UtcNow.AddSeconds(1),
        });
        await devicesContext.SaveChangesAsync();

        var response2 = await client.GetAsync($"{Paths.DevicesPath}/display?homeId={home}");
        var responseContent2 = await response2.Content.ReadAsStringAsync();
        var devices2 = JsonNode.Parse(responseContent2)!["items"]!.AsArray();

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, tileResponse.StatusCode);
        Assert.Single(devices);
        Assert.True(devices[0]!["tile"]!["value"]!.AsValue().GetValue<bool>());
        Assert.Single(devices2);
        Assert.False(devices2[0]!["tile"]!["value"]!.AsValue().GetValue<bool>());
    }
}
