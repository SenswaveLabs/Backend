using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.Mqtt.Devices.Presence;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class PresenceInterpretationMqttTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task RuntimeValuesForTileAndPresenceAreDifferent()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);

        var tileOperationId = await PostBooleanOperation(deviceId);
        var presenceOperationId = await PostBooleanOperation(deviceId);

        await PatchDeviceWithSwitchTile(deviceId, tileOperationId);
        await PatchDeviceWithBooleanPresence(deviceId, presenceOperationId);

        await SetPresenceOperationValue(tileOperationId, false);
        await SetPresenceOperationValue(presenceOperationId, true);

        await StartBrokerClient(broker);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}/display");
        var responseContent = await response.Content.ReadAsStringAsync();
        var device = JsonNode.Parse(responseContent)!.AsObject();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.False(device["tile"]!["value"]!.GetValue<bool>());
        Assert.True(device["presence"]!["value"]!.GetValue<bool>());
        Assert.Equal("BooleanOperation", device["presence"]!["type"]!.ToString());

        // Cleanup
        await StopBrokerClientInternal(broker);
    }

    [Fact]
    public async Task NoOperationValueIsValidForRuntimePresence()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);

        var tileOperationId = await PostBooleanOperation(deviceId);
        var presenceOperationId = await PostBooleanOperation(deviceId);

        await PatchDeviceWithSwitchTile(deviceId, tileOperationId);
        await PatchDeviceWithBooleanPresence(deviceId, presenceOperationId);

        await SetPresenceOperationValue(tileOperationId, true);
        // no value set for presenceOperationId

        await StartBrokerClient(broker);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.GetAsync($"{Paths.DevicesPath}/{deviceId}/display");
        var responseContent = await response.Content.ReadAsStringAsync();
        var device = JsonNode.Parse(responseContent)!.AsObject();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.True(device["tile"]!["value"]!.GetValue<bool>());
        Assert.Equal("BooleanOperation", device["presence"]!["type"]!.ToString());
        Assert.False(device["presence"]!["value"]!.GetValue<bool>());
        Assert.Null(device["presence"]!["lastSeenAtUtc"]);

        // Cleanup
        await StopBrokerClientInternal(broker);
    }
}
