using Senswave.TestInfrastructure.TestEnvironments.Mqtt;
using System.Net;
using System.Text.Json.Nodes;

namespace Senswave.Homes.EndTests.Mqtt.Homes;

[Collection("Mqtt E2E Tests Collection")]
[Trait("Collection", "MqttEndTest")]
public class GetHomeMqttTest(MqttTestEnvironment factory) : MqttFeatureTest(factory)
{
    [Fact]
    public async Task OwnerCanDisplayWithWorkinState()
    {
        // Arrange
        var client = CreateClient();
        var broker = await PostMqttBroker();
        var home = await PostAndPutBrokerForHome(broker);

        // Act
        await StartBrokerClient(broker);
        await AuthorizeClientAsUser(client);
        var response = await client.GetAsync($"{Paths.HomesPath}/{home}");
        var responseContent = await response.Content.ReadAsStringAsync();
        var parsingResult = JsonNode.Parse(responseContent)!.AsObject();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Working", parsingResult["dataSource"]!["state"]!.ToString());

        // Cleanup
        await StopBrokerClientInternal(broker);
    }
}
