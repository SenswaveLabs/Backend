using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Senswave.Users.Application.Auth.Google.Login;

namespace Senswave.Users.Application.Auth.Google.Services;

internal sealed class GoogleService(
    IOptionsSnapshot<AuthOptions> authOptions,
    ILogger<GoogleService> logger) : IGoogleService
{
    private readonly GoogleAuthOptions options = authOptions.Value.Google;

    public async Task<Result<ExternalUserDataModel>> RetriveExternalUserInfoFromJwt(string jwtToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                jwtToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [options.ClientId]
                });

            logger.LogInformation("[Subject: {subject}] Retrieved external user info successfully.", payload.Subject);

            return Result<ExternalUserDataModel>.Success(
                new ExternalUserDataModel
                {
                    Email =  payload.Email,
                    Subject = payload.Subject
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving google external user info.");
            return Result<ExternalUserDataModel>.Failure(GoogleAuthErrors.FailedToRetriveGoogleToken);
        }
    }

    public async Task<Result<ExternalUserDataModel>> RetriveExternalUserInfo(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenResponse = await RetrieveGoogleToken(code, cancellationToken);

            if (string.IsNullOrWhiteSpace(tokenResponse.IdToken))
            {
                logger.LogWarning("Google token exchange failed. No IdToken returned.");
                return Result<ExternalUserDataModel>.Failure(GoogleAuthErrors.FailedToRetriveGoogleToken);
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(
                tokenResponse.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [options.ClientId]
                });

            logger.LogInformation("[Subject: {subject}] Retrieved external user info successfully.", payload.Subject);

            return Result<ExternalUserDataModel>.Success(
                new ExternalUserDataModel
                {
                    Email =  payload.Email,
                    Subject = payload.Subject
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving google external user info.");
            return Result<ExternalUserDataModel>.Failure(GoogleAuthErrors.FailedToRetriveGoogleToken);
        }
    }

    private async Task<TokenResponse> RetrieveGoogleToken(string code, CancellationToken cancellationToken)
    {
        var googleAuthOptions = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret
            }
        };

        var flow = new GoogleAuthorizationCodeFlow(googleAuthOptions);

        var token = await flow.ExchangeCodeForTokenAsync(
            userId: "",
            code: code,
            redirectUri: "",
            taskCancellationToken: cancellationToken);

        return token;
    }
}
