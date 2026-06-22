using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Devices.Infrastructure;
using Senswave.Integration.Automations.TriggerOperationEvent;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using System.Text.Json.Nodes;

namespace Senswave.Automations.EndTests.Mqtt;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class TriggerAutomationFromWidgetTest(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task AutomationTriggeredFromWidget()
    {
        // Arrange
        const bool expectedResult = true;
        var client = CreateClient();

        var guids = await BaseAutomationGuids();

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        var clients = Services.GetRequiredService<IClientService>();

        var action = RequestFactory.CreateActionRequest(value: true).Serialize();

        await StartBrokerClient(guids["broker"]);
        var connection = clients.GetClient(guids["broker"]);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(guids["condition"]) };

        var results = new List<PostResultItemRequest>() { CreateResult(guids["result"], JsonValue.Create(expectedResult)) };

        await PostAutomation(guids["home"], conditions, results);

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync($"{Paths.WidgetsPath}/{guids["widget"]}/action", action);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Take time for automation service
        await Task.Delay(2000);

        // Assert
        await CheckIfBooleanAutomationResultSaved(guids["result"], expectedResult);

        Assert.True(connection.IsSuccess);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(testHarrness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarrness.Consumed.Select<PublishMessageRequest>().Any());

        //Cleanup
        await StopBrokerClientInternal(guids["broker"]);
    }

    [Fact]
    public async Task AutomationTriggeredFromWidgetWhenOnline()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(deviceId);
        var resultsOperation = await PostBooleanOperation(deviceId);
        var presenceOperation = await PostBooleanOperation(deviceId);

        await PatchDeviceWithBooleanPresence(deviceId, presenceOperation);
        var widgetId = await PostBooleanButtonWidget(conditionOperation);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(conditionOperation) };
        var results = new List<PostResultItemRequest>() { CreateResult(resultsOperation, JsonValue.Create(false)) };
        await PostAutomation(home, conditions, results);
        await StartBrokerClient(broker);

        var action = RequestFactory.CreateActionRequest(value: true).Serialize();

        using var assertScope = Factory.Services.CreateScope();
        var assertContext = assertScope.ServiceProvider.GetRequiredService<DevicesContext>();

        // Act
        await AuthorizeClientAsUser(client);
        await SetPresenceOperationValue(presenceOperation, true);
        var response = await client.PostAsync($"{Paths.WidgetsPath}/{widgetId}/action", action);

        await Task.Delay(4000);

        var resultOperation = await assertContext.Operations
            .Where(v => v.Id == resultsOperation)
            .FirstAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Single(resultOperation.Values);
        Assert.Equal(JsonValueKind.False, resultOperation.Values.First().Value.AsValue().GetValueKind());

        // Cleanup
        await StopBrokerClientInternal(broker);
    }

    [Fact]
    public async Task AutomationNotTriggeredFromWidgetWhenOffline()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(deviceId);
        var resultsOperation = await PostBooleanOperation(deviceId);
        var presenceOperation = await PostBooleanOperation(deviceId);

        await PatchDeviceWithBooleanPresence(deviceId, presenceOperation);
        var widgetId = await PostBooleanButtonWidget(conditionOperation);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(conditionOperation) };
        var results = new List<PostResultItemRequest>() { CreateResult(resultsOperation, JsonValue.Create(false)) };
        await PostAutomation(home, conditions, results);
        await StartBrokerClient(broker);

        var action = RequestFactory.CreateActionRequest(value: true).Serialize();

        using var scope = Factory.Services.CreateScope();
        var assertContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var messageBus = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();

        // This is value that would be sned by widget after processing but in mentime device tursn off so we cannot send it normally
        var request = new ExternalDeviceActionRequest
        {
            OperationsWithValues =
            [
                new TriggerOperationWithValue
                {
                    OperationId = resultsOperation,
                    Value = JsonValue.Create(false)
                }
            ]
        };

        // Act
        await AuthorizeClientAsUser(client);
        await SetPresenceOperationValue(presenceOperation, false);
        await Task.Delay(1000);

        await messageBus.Publish(request);

        await Task.Delay(4000);

        var resultOperation = await assertContext.Operations
            .Where(v => v.Id == resultsOperation)
            .FirstAsync();

        // Assert
        Assert.Empty(resultOperation.Values);

        // Cleanup
        await StopBrokerClientInternal(broker);
    }
}