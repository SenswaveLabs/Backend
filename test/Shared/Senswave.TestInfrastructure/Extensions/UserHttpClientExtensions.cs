using Senswave.TestInfrastructure.TestSetup.Models.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Senswave.TestInfrastructure.Extensions;

public static class UserHttpClientExtensions
{
    public static async Task PostLogin(this HttpClient client, string email, string password)
    {
        var request = new
        {
            email = email,
            password = password
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<AuthResponse>(responseContent) ?? new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    }
}
