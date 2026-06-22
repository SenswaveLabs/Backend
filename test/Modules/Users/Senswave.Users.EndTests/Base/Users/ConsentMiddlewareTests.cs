using Senswave.TestInfrastructure.TestEnvironments.Base;

namespace Senswave.Users.EndTests.Base.Users;

[Trait("Collection", "EndTest")]
public class ConsentMiddlewareTests(BaseTestEnvironment factory) : BaseFeatureTest(factory)
{
    [Fact]
    public async Task UserCanRetriveBasicInformationWithoutConsent()
    {
        // Arrange
        var (client, _, _)  = await CreateClientWithRoleWithoutConsent();

        // Act
        var response = await client.GetAsync(Paths.UsersPath);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserCanInvokeAuthEndpointsWithoutConsent()
    {
        // Arrange
        var (client, email, password) = await CreateClientWithRoleWithoutConsent();

        var request = new
        {
            Email = email,
            Password = password
        }.Serialize();

        // Act
        var response = await client.PostAsync($"{Paths.AuthPath}/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserCanInvokeLegalEndpointsWithoutConsent()
    {
        // Arrange
        var (client, _, _) = await CreateClientWithRoleWithoutConsent();

        // Act
        var response = await client.GetAsync($"{Paths.LegalPath}/terms");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ConsentAllowsToInvokeEndpoints()
    {
        // Arrange
        var (client, _, _)  = await CreateClientWithRoleWithoutConsent();

        // Act
        var failResponse = await client.GetAsync(Paths.HomesPath);

        await Tests.MakeConsent(client);

        var response = await client.GetAsync(Paths.BrokersPath);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, failResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
