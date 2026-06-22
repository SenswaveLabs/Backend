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
public class DeleteSubscribtionTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task SubscribtionIsRemovedFromWorkingClient()
    {
        // Arrange
        var client = CreateClient();
        var topic = $"test/{Guid.NewGuid()}";
        var brokerId = await PostMqttBroker();
        await AuthorizeClientAsUser(client);
        await StartBrokerClient(client, brokerId);

        using var scope = Factory.Server.Services.CreateScope();
        var testHarness = scope.ServiceProvider.GetRequiredService<ITestHarness>();

        // Act
        var subscribeRequest = RequestFactory.CreatePostSubscribtionRequest(topic).Serialize();
        var subscribeResponse = await client.PostAsync(Paths.SubscribtionsPath(brokerId), subscribeRequest);
        var subscribeContent = await subscribeResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<IdResponse>(subscribeContent);
        var subscriptionId = created!.Id;

        var deleteResponse = await client.DeleteAsync(Paths.SubscribtionPath(brokerId, subscriptionId));

        var getResponse = await client.GetAsync(Paths.SubscribtionsPath(brokerId));
        var listContent = await getResponse.Content.ReadAsStringAsync();
        var subscriptions = JsonSerializer.Deserialize<SubscribtionsDto>(listContent) ?? new();

        // Assert
        Assert.Equal(HttpStatusCode.Created, subscribeResponse.StatusCode);
        Assert.NotNull(created);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.True(await testHarness.Published.Any<UnsubscribeNotifications>());
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        await StopBrokerClientInternal(brokerId);
    }
}
