using Refit;

namespace Senswave.Presentation.Seed.Users.Clients;

public interface IUserClient
{
    [Post("/v1/users/consents")]
    Task MakeConsent([Authorize(scheme: "Bearer")] string token);
}
