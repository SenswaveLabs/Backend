using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Messaging.Interfaces;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Devices.Infrastructure;
using Senswave.Integration.Automations.TriggerOperationEvent;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.MessageReceivedFromDevice;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using System.Text.Json.Nodes;

namespace Senswave.Automations.EndTests.Mqtt;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class TriggerAutomationFromDeviceTest(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task TriggerAutomationFromDevice()
    {
        // Arrange
        const bool expectedResultValue = true;
        var client = CreateClient();

        var guids = await BaseAutomationGuids();

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        var clients = Services.GetRequiredService<IClientService>();

        await StartBrokerClient(guids["broker"]);
        var connection = clients.GetClient(guids["broker"]);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(guids["condition"]) };

        var results = new List<PostResultItemRequest>() { CreateResult(guids["result"], JsonValue.Create(expectedResultValue)) };

        await PostAutomation(guids["home"], conditions, results);

        // Act
        using var scope = Factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();
        var dataSourceContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var operation = await dataSourceContext.Operations
            .Include(x => x.DataReference)
            .Where(x => x.Id == guids["condition"])
            .FirstOrDefaultAsync();

        Assert.NotNull(operation);

        await messageBus.Publish(new MessageReceivedFromDeviceEvent()
        {
            BrokerId = guids["broker"],
            SubscribtionId = operation.DataReference!.DataSourceDataReferenceId,
            Payload = "{\"value\": true}"
        });

        // Take time for automation service
        await Task.Delay(2000);
        await AuthorizeClientAsUser(client);

        // Assert
        await CheckIfBooleanAutomationResultSaved(guids["result"], expectedResultValue);


        Assert.True(connection.IsSuccess);
        Assert.True(testHarrness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarrness.Consumed.Select<PublishMessageRequest>().Any());


        //Cleanup
        await StopBrokerClientInternal(guids["broker"]);
    }

    [Fact]
    public async Task AutomationTriggeredFromDeviceWhenOnline()
    {
        // Arrange
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(deviceId);
        var resultsOperation = await PostBooleanOperation(deviceId);
        var presenceOperation = await PostBooleanOperation(deviceId);

        await PatchDeviceWithBooleanPresence(deviceId, presenceOperation);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(conditionOperation) };
        var results = new List<PostResultItemRequest>() { CreateResult(resultsOperation, JsonValue.Create(false)) };
        await PostAutomation(home, conditions, results);
        await StartBrokerClient(broker);

        using var scope = Factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();
        var dataSourceContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();
        var conditionOp = await dataSourceContext.Operations
            .Include(x => x.DataReference)
            .Where(x => x.Id == conditionOperation)
            .FirstOrDefaultAsync();

        Assert.NotNull(conditionOp);

        // Act
        await SetPresenceOperationValue(presenceOperation, true);
        await messageBus.Publish(new MessageReceivedFromDeviceEvent()
        {
            BrokerId = broker,
            SubscribtionId = conditionOp.DataReference!.DataSourceDataReferenceId,
            Payload = "{\"value\": true}"
        });

        await Task.Delay(4000);

        var resultOp = await dataSourceContext.Operations
            .Where(v => v.Id == resultsOperation)
            .FirstAsync();

        // Assert
        Assert.Single(resultOp.Values);
        Assert.Equal(JsonValueKind.False, resultOp.Values.First().Value.AsValue().GetValueKind());

        // Cleanup
        await StopBrokerClientInternal(broker);
    }

    [Fact]
    public async Task AutomationNotTriggeredFromDeviceWhenOffline()
    {
        // Arrange
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);
        var deviceId = await PostDevice(home);
        var conditionOperation = await PostBooleanOperation(deviceId);
        var resultsOperation = await PostBooleanOperation(deviceId);
        var presenceOperation = await PostBooleanOperation(deviceId);

        await PatchDeviceWithBooleanPresence(deviceId, presenceOperation);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(conditionOperation) };
        var results = new List<PostResultItemRequest>() { CreateResult(resultsOperation, JsonValue.Create(false)) };
        await PostAutomation(home, conditions, results);
        await StartBrokerClient(broker);

        using var scope = Factory.Services.CreateScope();
        var messageBus = scope.ServiceProvider.GetRequiredService<IPublishMessageBus>();
        var assertContext = scope.ServiceProvider.GetRequiredService<DevicesContext>();

        // This is the value that would be sent by the automation after processing,
        // but in the meantime the device turns off so we cannot send it normally
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
        await SetPresenceOperationValue(presenceOperation, false);
        await Task.Delay(1000);

        await messageBus.Publish(request);

        await Task.Delay(4000);

        var resultOp = await assertContext.Operations
            .Where(v => v.Id == resultsOperation)
            .FirstAsync();

        // Assert
        Assert.Empty(resultOp.Values);

        // Cleanup
        await StopBrokerClientInternal(broker);
    }
}