using Senswave.TestInfrastructure.TestEnvironments.Base;
using System.Text.Json.Nodes;

namespace Senswave.Devices.EndTests.Base.Operations.Types;

[Trait("Collection", "EndTest")]
public class IntegerOperationTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task CanCreateIntegerOperation()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var home = await PostHomeWithBroker();
        var device = await PostDevice(home);
        var request = new JsonObject()
        {
            ["deviceId"] = device,
            ["name"] = Guid.NewGuid().ToString(),
            ["type"] = "Integer",
            ["configuration"] = new JsonObject()
            {
                ["jsonNames"] = new JsonArray("value"),
                ["isJson"] = true,
                ["min"] = 0,
                ["max"] = 100
            },
            ["topic"] = Guid.NewGuid().ToString()
        };

        // Act
        await AuthorizeClientAsUser(client);
        var response = await client.PostAsync(Paths.OperationsPath, request.Serialize());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
