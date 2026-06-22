using Microsoft.AspNetCore.Identity;
using Senswave.Users.Application.Auth.Google.Services;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Enums;
using Senswave.Users.Domain.Interfaces;


namespace Senswave.Users.Application.Auth.Google.Login;


internal sealed class GoogleAuthHandler(
    UserManager<User> userManager,
    SignInManager<User> singInManager,
    IUserService externalUserService,
    IGoogleService googleService,
    ILogger<GoogleAuthHandler> logger) : ICommandHandler<GoogleAuthCommand>
{
    public async Task<Result> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var externalUserResult = await googleService.RetriveExternalUserInfo(request.Code, cancellationToken);

            if (externalUserResult.IsFailure)
            {
                logger.LogWarning("Failed to retrieve google external user info.");
                return Result.Failure(externalUserResult.Errors);
            }

            var user = await userManager.FindByEmailAsync(externalUserResult.Data.Email);

            if (user is null)
            {
                var registerResult = await externalUserService.Register(
                    ExternalProvider.Google,
                    externalUserResult.Data.Email,
                    externalUserResult.Data.Subject,
                    cancellationToken);

                if (registerResult.IsFailure)
                {
                    logger.LogWarning("Failed to register google user.");
                    return Result.Failure(registerResult.Errors);
                }

                user = registerResult.Data;
            }
            else
            {
                var registerResult = await externalUserService.LinkExtenralProvider(
                    ExternalProvider.Google,
                    user,
                    externalUserResult.Data.Subject,
                    cancellationToken);

                if (registerResult.IsFailure)
                {
                    logger.LogWarning("Failed to register google user.");
                    return Result.Failure(registerResult.Errors);
                }
            }

            singInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
            await singInManager.SignInAsync(user, false);

            logger.LogInformation("[User: {user}] User registered and logged successfully.", user.Id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during google authentication.");

            return Result.Failure(GoogleAuthErrors.FailedToAuthenticate);
        }
    }
}
