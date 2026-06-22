using System.Net.Http.Headers;
using System.Text.Json;

namespace Senswave.TestInfrastructure.Extensions;

public static class Extensions
{
    public static HttpContent Serialize<T>(this T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var content = new StringContent(json);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return content;
    }

    public static async Task<T> Deserialize<T>(this HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<T>(responseContent);
        return responseObject!;
    }
}
