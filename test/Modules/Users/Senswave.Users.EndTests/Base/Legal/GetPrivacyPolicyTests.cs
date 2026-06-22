using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.Users.Infrastructure.Services;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senswave.Users.EndTests.Base.Legal;

[Trait("Collection", "EndTest")]
public class GetPrivacyPolicyTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    private const string CurrentVersion = "1.0.1";

    [Fact]
    public async Task AnyoneCanGetPrivacyPolicy()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var version = LegalTestsHelpers.GetLatestVersion(LagalPpAndTcService.PrivacyPolicyDirPath);

        // Act
        await Task.Delay(200);
        var response = await client.GetAsync($"{Paths.LegalPath}/privacy");
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonObject>(content);

        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Response code: {response.StatusCode}");
        Assert.Contains("# Privacy Policy", content);
        Assert.NotNull(data);
        Assert.Equal(version, data["version"]!.ToString().Trim()); // dir - db
        Assert.Equal(CurrentVersion, data["version"]!.ToString().Trim()); // hardcoded - db
    }
}
