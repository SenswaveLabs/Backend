using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Integration.DataSource.BrokerConnection.Notifications;
using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using Senswave.TestInfrastructure.TestSetup.Models.DataSources;

namespace Senswave.DataSources.EndTests.Mqtt.Subscribtions;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class CreateSubscribtionTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task SubscribtionIsCreatedInWorkingClient()
    {
        // Arrange
        var client = CreateClient();
        var topic = $"test/{Guid.NewGuid()}";

        using var scope = Factory.Server.Services.CreateScope();
        var testHarness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        await testHarness.Start();

        var brokerId = await PostMqttBroker();
        await AuthorizeClientAsUser(client);
        await StartBrokerClient(client, brokerId);

        var request = RequestFactory.CreatePostSubscribtionRequest(topic).Serialize();

        // Act
        var response = await client.PostAsync(Paths.SubscribtionsPath(brokerId), request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<IdResponse>(responseContent);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.True(await testHarness.Published.Any<SubscribeNotification>());

        var getResponse = await client.GetAsync(Paths.SubscribtionsPath(brokerId));
        var subscribtionsContent = await getResponse.Content.ReadAsStringAsync();
        var subscribtions = JsonSerializer.Deserialize<SubscribtionsDto>(subscribtionsContent) ?? new();

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Contains(subscribtions.Items, x => x.Topic == topic);

        await StopBrokerClientInternal(brokerId);
    }
}
