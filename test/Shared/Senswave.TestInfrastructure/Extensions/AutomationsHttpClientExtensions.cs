using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Models.Automations;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using System.Text.Json;

namespace Senswave.TestInfrastructure.Extensions;

public static class AutomationsHttpClientExtensions
{
    public static async Task<Guid> PostAutomation(this HttpClient client, Guid homeId, IList<PostConditionItemRequest> conditions,
        IList<PostResultItemRequest> results)
    {
        var request = new CreateAutomationRequest
        {
            HomeId = homeId,
            Name = Guid.NewGuid().ToString()[1..30],
            Icon = "gear",
            ConditionConnector = "And",
            Conditions = conditions,
            Results = results
        };

        var response = await client.PostAsync(Paths.AutomationPath, request.Serialize());
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var responseDto = JsonSerializer.Deserialize<IdResponse>(content);

        return responseDto!.Id;
    }
}
