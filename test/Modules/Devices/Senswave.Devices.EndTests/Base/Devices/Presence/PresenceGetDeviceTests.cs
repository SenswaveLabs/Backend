using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Devices;

namespace Senswave.Devices.EndTests.Base.Devices.Presence;

[Trait("Collection", "EndTest")]
public class PresenceGetDeviceTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task DefaultPresenceReturnedWhenNoPresenceConfigured()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var device = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Default", device.Presence.Type);
        Assert.Null(device.Presence.OperationId);
    }

    [Fact]
    public async Task BooleanPresenceTypeAndOperationReturnedInGetDevice()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithBooleanPresence(deviceId, operationId);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var device = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("BooleanOperation", device.Presence.Type);
        Assert.Equal(operationId.ToString(), device.Presence.OperationId);
    }

    [Fact]
    public async Task DefaultPresenceReturnedAfterPresenceRevoked()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithBooleanPresence(deviceId, operationId);

        var revokeRequest = RequestFactory.CreatePatchDevice(presenceType: "Default");
        await client.PatchAsync($"{Paths.DevicesPath}/{deviceId}", revokeRequest.Serialize());

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var device = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Default", device.Presence.Type);
        Assert.Null(device.Presence.OperationId);
    }

    [Fact]
    public async Task DefaultPresenceReturnedWhenPresenceEntityMissing()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);

        using var scope = Factory.Server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var presence = await context.Set<DevicePresence>().FirstOrDefaultAsync(x => x.DeviceId == deviceId);
        if (presence != null)
        {
            context.Set<DevicePresence>().Remove(presence);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}");
        var data = await response.Content.ReadAsStringAsync();
        var device = JsonSerializer.Deserialize<GetDeviceResponse>(data)!;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Default", device.Presence.Type);
        Assert.Null(device.Presence.OperationId);
    }
}
