using Senswave.TestInfrastructure.TestEnvironments.Mqtt;

namespace Senswave.DataSources.EndTests.Mqtt.Clients;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class RestartClientTests(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    // Restarting the client is started too fast.

    //[Fact]
    //public async Task OwnerCanRestartConnection()
    //{
    //    // Arrange
    //    var client = CreateClient();
    //    var clientService = Services.GetRequiredService<IClientService>();
    //    var testHarness = Services.GetRequiredService<ITestHarness>()!;

    //    // Act
    //    var brokerId = await PostMqttBroker();
    //    await AuthorizeClientAsUser(client);
    //    await StartBrokerClient(brokerId);

    //    await Task.Delay(1000);

    //    var clientResult = clientService.GetClient(brokerId);
    //    var connected = clientResult.Data.IsConnected;

    //    await testHarness.Start();
    //    var stopResult = await clientResult.Data.Stop();
    //    var disconnected = clientResult.Data.IsConnected;
    //    var clientNotRemoved = clientService.GetClient(brokerId);
    //    var restartResponse = await client.PatchAsync($"{}/{brokerId}/restart", null);
    //    var currentClient = clientService.GetClient(brokerId);

    //    await Task.Delay(1000);

    //    // Assert
    //    Assert.True(clientResult.IsSuccess);
    //    Assert.True(connected);
    //    Assert.True(stopResult.IsSuccess);
    //    Assert.False(disconnected);
    //    Assert.True(clientNotRemoved.IsSuccess, "Client Suddenly Removed");
    //    var content = await restartResponse.Content.ReadAsStringAsync();
    //    Assert.True(HttpStatusCode.NoContent == restartResponse.StatusCode, $"ClientConnected after restart: {currentClient.Data.IsConnected} Message: {content}");
    //    Assert.True(currentClient.IsSuccess);
    //    Assert.True(currentClient.Data.IsConnected);
    //    Assert.True(await testHarness.Published.Any<RestartClientRequest>());

    //    // Cleanup
    //    await StopBrokerClientInternal(brokerId);
    //}
}
