using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.MessageOperationProcessed;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.ModuleEvents.Operations;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class OperationTestsNoEvent(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task EventNotFiredOnWidgetAction()
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
        Assert.False(testHarrness.IsMessageInSystem<ProcessedOperationTriggerAutomationEvent>(), "Event was published and should not be.");

        //Cleanup
        await StopBrokerClientInternal(broker);
    }

    [Fact]
    public async Task EventNotFiredOnDeviceTileAction()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId, false);
        var widgetId = await PostBooleanButtonWidget(operationId);

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        var clients = Services.GetRequiredService<IClientService>();

        var action = RequestFactory.CreateActionRequest(value: true).Serialize();

        await StartBrokerClient(broker);
        var connection = clients.GetClient(broker);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync($"{Paths.WidgetsPath}/{widgetId}/action", action);

        await Task.Delay(1000);

        // Assert
        Assert.True(connection.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(testHarrness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarrness.Consumed.Select<PublishMessageRequest>().Any());
        Assert.False(testHarrness.IsMessageInSystem<ProcessedOperationTriggerAutomationEvent>(), "Event was published and should not be.");

        //Cleanup
        await StopBrokerClientInternal(broker);
    }
}
