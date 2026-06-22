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
public class DisabledAutomationTest(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task DisabledAutomationCanNotBeTriggered()
    {
        // Arrange
        const bool expectedResultValue = false;
        var client = CreateClient();

        var guids = await BaseAutomationGuids();

        var testHarrness = Factory.Server.Services.GetService<ITestHarness>()!;
        var clients = Services.GetRequiredService<IClientService>();

        var conditionAction = RequestFactory.CreateActionRequest(value: true).Serialize();

        var resultAction = RequestFactory.CreateActionRequest(value: expectedResultValue).Serialize();
        var resultWidget = await PostSwitchWidget(guids["result"]);

        await StartBrokerClient(guids["broker"]);
        var connection = clients.GetClient(guids["broker"]);

        var conditions = new List<PostConditionItemRequest>() { CreateCondition(guids["condition"]) };

        var results = new List<PostResultItemRequest>() { CreateResult(guids["result"], JsonValue.Create(true)) };

        var automationId = await PostAutomation(guids["home"], conditions, results);
        await AuthorizeClientAsUser(client);
        await DisableAutomation(client, automationId);

        // Act
        var resultResponse = await client.PostAsync($"{Paths.WidgetsPath}/{resultWidget}/action", resultAction);
        var conditionResponse = await client.PostAsync($"{Paths.WidgetsPath}/{guids["widget"]}/action", conditionAction);
        Assert.Equal(HttpStatusCode.NoContent, conditionResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, resultResponse.StatusCode);

        // Take time for automation service
        Thread.Sleep(2000);

        // Assert
        await CheckIfBooleanAutomationResultSaved(guids["result"], expectedResultValue);
        Assert.True(connection.IsSuccess);
        Assert.True(testHarrness.Consumed.Select<PublishMessageToDeviceRequest>().Any());
        Assert.True(testHarrness.Consumed.Select<PublishMessageRequest>().Any());

        //Cleanup
        await StopBrokerClientInternal(guids["broker"]);
    }

    private async Task DisableAutomation(HttpClient client, Guid automationId)
    {
        var request = new JsonObject { ["isEnabled"] = false };

        var response = await client.PutAsync($"{Paths.AutomationPath}/{automationId}/state", request.Serialize());
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}