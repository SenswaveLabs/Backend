namespace Senswave.Users.Application.Auth.Google.Services;

public interface IGoogleService
{
    Task<Result<ExternalUserDataModel>> RetriveExternalUserInfoFromJwt(string jwtToken, CancellationToken cancellationToken = default);
    Task<Result<ExternalUserDataModel>> RetriveExternalUserInfo(string idToken, CancellationToken cancellationToken = default);
}
