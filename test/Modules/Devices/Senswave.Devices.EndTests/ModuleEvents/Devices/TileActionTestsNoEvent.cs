using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.Devices;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.ModuleEvents.Devices;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class TileActionTestsNoEvent(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task EventNotFiredOnAction()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId, false);
        await PatchDeviceWithSwitchTile(deviceId, operationId);

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        var clients = Services.GetRequiredService<IClientService>();

        var tileAction = new JsonObject
        {
            ["value"] = true
        }.Serialize();

        await StartBrokerClient(broker);
        var connection = clients.GetClient(broker);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync($"{Paths.DevicesPath}/{deviceId}/tile/action", tileAction);

        await Task.Delay(1000);

        // Assert
        Assert.True(connection.IsSuccess);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(testHarrness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarrness.Consumed.Select<PublishMessageRequest>().Any());
        Assert.False(testHarrness.IsMessageInSystem<DeviceTileActionEvent>(), "Event was published and should not be.");

        //Cleanup
        await StopBrokerClientInternal(broker);
    }
}
