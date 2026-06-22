using Senswave.Devices.Domain.Devices.Enums;
using Senswave.Devices.Domain.Devices.Extensions;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Devices;

namespace Senswave.Devices.EndTests.Base.Devices.TileTypes.Display;

[Trait("Collection", "EndTest")]
public class GetDeviceTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task DisplayTileTypeAndOperationReturnedInGetDevice()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostNumberOperation(deviceId);
        await PatchDeviceWithDisplayTile(deviceId, operationId);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var device = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(deviceId, device.Id);
        Assert.Equal(DeviceTileType.Display, device.Tile.Type.ToDeviceTileType());
        Assert.Null(device.Tile.OperationId);
        Assert.Equal(operationId.ToString(), device.Tile.DisplayableOperationId);
    }

    [Fact]
    public async Task DisplayTileOperationIdClearedAfterReset()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostNumberOperation(deviceId);
        await PatchDeviceWithDisplayTile(deviceId, operationId);

        var revokeRequest = RequestFactory.CreateAssignOperationToDevice(
            operationId: Guid.Empty.ToString(),
            type: "Default").Serialize();
        await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", revokeRequest);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var device = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(DeviceTileType.Default, device.Tile.Type.ToDeviceTileType());
        Assert.Null(device.Tile.OperationId);
        Assert.Null(device.Tile.DisplayableOperationId);
    }
}
