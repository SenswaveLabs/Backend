using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Devices.Domain.Devices.Entities;
using Senswave.Devices.Infrastructure;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.Base.Devices.Presence;

[Trait("Collection", "EndTest")]
public class PresenceDisplayDevicesTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task DefaultPresenceReturnedInDeviceList()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        await PostDevice(home);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/display?homeId={home}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var devices = JsonNode.Parse(responseContent)!["items"]!.AsArray();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(devices);
        Assert.Equal("Default", devices[0]!["presence"]!["type"]!.GetValue<string>());
        Assert.Null(devices[0]!["presence"]!["value"]);
        Assert.Null(devices[0]!["presence"]!["lastSeenAtUtc"]);
    }

    [Fact]
    public async Task BooleanPresenceTypeReturnedInDeviceList()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await AuthorizeClientAsUser(client);

        var home = await PostHomeWithBroker();
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithBooleanPresence(deviceId, operationId);

        // Act
        var response = await client.GetAsync($"{Paths.DevicesPath}/display?homeId={home}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var devices = JsonNode.Parse(responseContent)!["items"]!.AsArray();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(devices);
        Assert.Equal("BooleanOperation", devices[0]!["presence"]!["type"]!.GetValue<string>());
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
        var response = await client.GetAsync($"{Paths.DevicesPath}/display?homeId={home}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var devices = JsonNode.Parse(responseContent)!["items"]!.AsArray();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(devices);
        Assert.Equal("Default", devices[0]!["presence"]!["type"]!.GetValue<string>());
        Assert.Null(devices[0]!["presence"]!["value"]);
        Assert.Null(devices[0]!["presence"]!["lastSeenAtUtc"]);
    }
}
