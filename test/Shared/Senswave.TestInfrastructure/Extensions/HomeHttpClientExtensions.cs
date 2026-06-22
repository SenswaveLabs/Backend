using Senswave.TestInfrastructure.TestEnvironments.Common;
using Senswave.TestInfrastructure.TestSetup.Models;
using Senswave.TestInfrastructure.TestSetup.Models.Common;
using Senswave.TestInfrastructure.TestSetup.Models.Homes;
using System.Text.Json;

namespace Senswave.TestInfrastructure.Extensions;

public static class HomeHttpClientExtensions
{
    public static async Task<Guid> PostHome(this HttpClient client, int longitude = 1, int latitude = 1)
    {
        var request = RequestFactory.CreatePostHome(
            name: Guid.NewGuid().ToString()[..29],
            icon: "default-icon",
            latitude: latitude,
            longitude: longitude)
            .Serialize();

        var response = await client.PostAsync(Paths.HomesPath, request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);

        return postResponse!.Id;
    }

    public static async Task<Guid> PostRoom(this HttpClient client, Guid homeId, string name)
    {
        var request = RequestFactory.CreatePostRoom(name: name)
            .Serialize();

        var response = await client.PostAsync(Paths.RoomsPath(homeId), request);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var postResponse = JsonSerializer.Deserialize<IdResponse>(responseContent);

        return postResponse!.Id;
    }

    public static async Task<Guid> PutDataSourceForHome(this HttpClient client, Guid brokerId, Guid homeId)
    {
        var request = RequestFactory.CreatePutHomeBroker(brokerId: brokerId)
            .Serialize();

        var response = await client.PutAsync($"{Paths.HomesPath}/{homeId}/datasource", request);
        response.EnsureSuccessStatusCode();

        return homeId;
    }

    public static async Task<InviteFriendResponse> PostHomeInvitation(this HttpClient client, Guid homeId, string email, string sharingType = "Manage")
    {
        var request = RequestFactory.CreateFriendInvitation(homeId, email, sharingType);

        var response = await client.PostAsync($"{Paths.HomesPath}/sharings", request.Serialize());
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var responseDto = JsonSerializer.Deserialize<InviteFriendResponse>(content);

        return responseDto!;
    }

    public static async Task AcceptHomeInvitation(this HttpClient acceptor, string password)
    {
        var request = RequestFactory.CreateAcceptInvitation(password);
        var response = await acceptor.PutAsync($"{Paths.HomesPath}/sharings", request.Serialize());
        response.EnsureSuccessStatusCode();
    }
}
