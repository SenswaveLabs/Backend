using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using System.Text.Json;

namespace Senswave.TestInfrastructure.Extensions;

public static class DataSourceHttpClientExtensions
{
    public static async Task<Guid> PostDataSourceBroker(this HttpClient client)
    {
        Random random = new();
        var placeholder = Guid.NewGuid().ToString();

        var request = RequestFactory.CreatePostBrokerRequest(
            name: placeholder[..29],
            clientName: Guid.NewGuid().ToString(),
            url: placeholder,
            port: random.Next(5000, 10000),
            protocolVersion: "MqttV5").Serialize();

        var response = await client.PostAsync(Paths.BrokersPath, request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);

        return postResponse!.Id;
    }
}
