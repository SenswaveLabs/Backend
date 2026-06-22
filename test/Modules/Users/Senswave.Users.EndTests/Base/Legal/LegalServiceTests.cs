using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.TestInfrastructure.TestEnvironments.Base;
using Senswave.Users.Infrastructure;
using Senswave.Users.Infrastructure.Services;

namespace Senswave.Users.EndTests.Base.Legal;

[Trait("Collection", "EndTest")]
public class LegalServiceTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task ValidPrivacyPolicyInDatabase()
    {
        // Arrange
        using var scope = Factory.Server.Services.CreateScope();
        var client = CreateUnauthorizedClient();

        // Act
        var context = scope.ServiceProvider.GetRequiredService<UsersContext>();

        var latestInDir = LegalTestsHelpers.GetLatestVersion(LagalPpAndTcService.PrivacyPolicyDirPath);

        var latest = await context.PrivacyPolicies
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(latest);
        Assert.Equal(latestInDir, latest.Version);
        Assert.Contains("# Privacy Policy", latest.Content);
    }

    [Fact]
    public async Task ValidTermsInDatbase()
    {
        // Arrange
        using var scope = Factory.Server.Services.CreateScope();
        var client = CreateUnauthorizedClient();

        // Act
        var context = scope.ServiceProvider.GetRequiredService<UsersContext>();

        var latestInDir = LegalTestsHelpers.GetLatestVersion(LagalPpAndTcService.TermsAndConditionsDirPath);

        var latest = await context.TermsAndConditions
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(latest);
        Assert.Equal(latestInDir, latest.Version);
        Assert.Contains("# Terms and Conditions", latest.Content);
    }
}
