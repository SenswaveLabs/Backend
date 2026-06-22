using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using Senswave.DataSources.Infrastructure;
using Senswave.Devices.Infrastructure;
using Senswave.Integration.DataTransfer.Devices;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.ModuleEvents.Devices;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class PresenceTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task EventFiredOnIncomingAction()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var operationId = await PostBooleanOperation(deviceId);
        await PatchDeviceWithBooleanPresence(deviceId, operationId);

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
            .WithPayload(new JsonObject { ["value"] = true }.ToJsonString())
            .Build();

        // Act
        await brokerClient.PublishAsync(message, default);
        await Task.Delay(1000);

        // Assert
        Assert.True(testHarrness.Published.Select<DevicePresenceEvent>().Any(), "Presence event was not published.");

        // Cleanup
        await MqttHelpers.Cleanup(brokerClient);
    }
}
