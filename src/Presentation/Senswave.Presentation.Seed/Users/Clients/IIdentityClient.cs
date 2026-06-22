using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity.Data;
using Refit;

namespace Senswave.Presentation.Seed.Users.Clients;

public interface IIdentityClient
{
    [Post("/v1/auth/register")]
    Task Register(RegisterRequest request);

    [Post("/v1/auth/login")]
    Task<AccessTokenResponse> Login(LoginRequest request);
}
