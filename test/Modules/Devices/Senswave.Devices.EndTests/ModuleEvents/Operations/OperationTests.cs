using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Infrastructure;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.Devices;
using Senswave.Integration.DataTransfer.MessageOperationProcessed;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.ModuleEvents.Operations;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class OperationTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task EventsAreFiredOnDeviceTileAction()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithSwitchTile(deviceId, operationId);

        var testHarness = Factory.Server.Services.GetService<ITestHarness>()!;
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
        Assert.True(testHarness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarness.Consumed.Select<PublishMessageRequest>().Any());
        Assert.True(testHarness.IsMessageInSystem<ProcessedOperationTriggerAutomationEvent>(), "ProcessedOperationTriggerAutomationEvent was not published.");

        // Cleanup
        await StopBrokerClientInternal(broker);
    }

    [Fact]
    public async Task EventsAreFiredOnWidgetAction()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        var widgetId = await PostBooleanButtonWidget(operationId);

        var testHarness = Factory.Server.Services.GetService<ITestHarness>()!;
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
        Assert.True(testHarness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarness.Consumed.Select<PublishMessageRequest>().Any());
        Assert.True(testHarness.IsMessageInSystem<ProcessedOperationTriggerAutomationEvent>(), "ProcessedOperationTriggerAutomationEvent was not published.");

        // Cleanup
        await StopBrokerClientInternal(broker);
    }

    [Fact]
    public async Task EventsAreFiredOnIncomingAction()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithSwitchTile(deviceId, operationId);
        var widgetId = await PostBooleanButtonWidget(operationId);

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        await StartBrokerClient(broker);

        using var scope = Factory.Server.Services.CreateScope();
        var devicesContext = scope.ServiceProvider.GetService<DevicesContext>()!;
        var dataSourcesContext = scope.ServiceProvider.GetService<DataSourcesContext>()!;

        var operation = await devicesContext.Operations
            .Include(x => x.DataReference)
            .FirstAsync(x => x.Id == operationId);

        var subscribtion = await dataSourcesContext.Subscribtions
            .FirstAsync(x => x.Id == operation.DataReference!.DataSourceDataReferenceId);

        var brokerClient = await StartSepareateBrokerClient();

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(subscribtion.Topic)
            .WithPayload(new JsonObject { ["value"] = false }.ToJsonString())
            .Build();

        // Act
        await brokerClient.PublishAsync(message, default);
        await Task.Delay(1000);

        // Assert
        Assert.True(testHarrness.Published.Select<ProcessedOperationTriggerAutomationEvent>().Any(), "ProcessedOperationTriggerAutomationEvent event was not published.");
        Assert.True(testHarrness.Published.Select<WidgetActionEvent>().Any(), "WidgetActionEvent event was not published.");
        Assert.True(testHarrness.Published.Select<DeviceTileActionEvent>().Any(), "DeviceTileActionEvent event was not published.");

        // Cleanup
        await MqttHelpers.Cleanup(brokerClient);
    }

    [Fact]
    public async Task EventsAreFiredOnIncomingActionWithNoUserEventConfiguration()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId, false);
        await PatchDeviceWithSwitchTile(deviceId, operationId);
        var widgetId = await PostBooleanButtonWidget(operationId);

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        await StartBrokerClient(broker);

        using var scope = Factory.Server.Services.CreateScope();
        var devicesContext = scope.ServiceProvider.GetService<DevicesContext>()!;
        var dataSourcesContext = scope.ServiceProvider.GetService<DataSourcesContext>()!;

        var operation = await devicesContext.Operations
            .Include(x => x.DataReference)
            .FirstAsync(x => x.Id == operationId);

        var subscribtion = await dataSourcesContext.Subscribtions
            .FirstAsync(x => x.Id == operation.DataReference!.DataSourceDataReferenceId);

        var brokerClient = await StartSepareateBrokerClient();

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(subscribtion.Topic)
            .WithPayload(new JsonObject { ["value"] = false }.ToJsonString())
            .Build();

        // Act
        await brokerClient.PublishAsync(message, default);
        await Task.Delay(1000);

        // Assert
        Assert.True(testHarrness.Published.Select<ProcessedOperationTriggerAutomationEvent>().Any(), "ProcessedOperationTriggerAutomationEvent event was not published.");
        Assert.True(testHarrness.Published.Select<WidgetActionEvent>().Any(), "WidgetActionEvent event was not published.");
        Assert.True(testHarrness.Published.Select<DeviceTileActionEvent>().Any(), "DeviceTileActionEvent event was not published.");

        // Cleanup
        await MqttHelpers.Cleanup(brokerClient);
    }
}
