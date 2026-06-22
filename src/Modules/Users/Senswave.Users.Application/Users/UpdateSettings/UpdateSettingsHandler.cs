using Senswave.Users.Domain.Enums;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Application.Users.UpdateSettings;

internal sealed class UpdateSettingsHandler(
    ICommandUserRepository repository,
    ILogger<UpdateSettingsHandler> logger) : ICommandHandler<UpdateSettingsCommand>
{
    public async Task<Result> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUser(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogError("[User: {userId}] User not found", request.UserId);
            return Result.Failure([UpdateSettingsError.UserNotFound]);
        }

        if (request.Language != Language.Empty)
            user.Language = request.Language;

        if (request.Theme != Theme.Empty)
            user.Theme = request.Theme;

        var updateResult = await repository.UpdateUser(user, cancellationToken);

        if (!updateResult)
        {
            logger.LogError("[User: {userId}] Failed to update user settings", request.UserId);
            return Result.Failure(updateResult.Errors);
        }

        logger.LogInformation("[User: {userId}] User settings updated successfully", request.UserId);
        return Result.Success();
    }
}
