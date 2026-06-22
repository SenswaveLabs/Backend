using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.Devices;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models;

namespace Senswave.Devices.EndTests.ModuleEvents.Widgets;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class ActionTestsNoEvent(MqttTestEnvironment factory) : MqttFeatureTest(factory)
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
        Assert.False(testHarrness.IsMessageInSystem<WidgetActionEvent>(), "Event was published and should not be.");

        //Cleanup
        await StopBrokerClientInternal(broker);
    }
}
