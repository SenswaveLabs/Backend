using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Senswave.DataSources.BrokerConnection.Interfaces;
using Senswave.Integration.DataSource.BrokerConnection.PublishMessage;
using Senswave.Integration.DataTransfer.PublishMessageToDevice;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using System.Text.Json.Nodes;


namespace Senswave.Automations.EndTests.Mqtt;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class TriggerAutomationFromAutomationTest(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task AutomationShouldNotTriggerOtherAutomation()
    {
        // Arrange
        const bool firstAutomationExpectedResult = true;
        const bool secondAutomationTriggeredResult = true;

        var client = CreateClient();

        var guids = await BaseAutomationGuids();

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        var clients = Services.GetRequiredService<IClientService>();

        var conditionAction = RequestFactory.CreateActionRequest(value: true).Serialize();

        await StartBrokerClient(guids["broker"]);
        var connection = clients.GetClient(guids["broker"]);

        var conditions1 = new List<PostConditionItemRequest>() { CreateCondition(guids["condition"]) };

        var results1 = new List<PostResultItemRequest>() { CreateResult(guids["result"], JsonValue.Create(firstAutomationExpectedResult)) };

        var resultsOperation2 = await PostBooleanOperation(guids["device"]);

        var conditions2 = new List<PostConditionItemRequest>() { CreateCondition(guids["result"]) };

        var results2 = new List<PostResultItemRequest>() { CreateResult(resultsOperation2, JsonValue.Create(secondAutomationTriggeredResult)) };

        await PostAutomation(guids["home"], conditions1, results1);
        await PostAutomation(guids["home"], conditions2, results2);

        // Act
        await AuthorizeClientAsUser(client);
        var conditionResponse = await client.PostAsync($"{Paths.WidgetsPath}/{guids["widget"]}/action", conditionAction);
        Assert.Equal(HttpStatusCode.NoContent, conditionResponse.StatusCode);

        await Task.Delay(4000);

        // Assert
        await CheckIfBooleanAutomationResultSaved(guids["result"], firstAutomationExpectedResult);
        await OperationNotTriggered(resultsOperation2);

        Assert.True(connection.IsSuccess);
        Assert.True(testHarrness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarrness.Consumed.Select<PublishMessageRequest>().Any());

        //Cleanup
        await StopBrokerClientInternal(guids["broker"]);
    }
}