using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Application.Users.GetUser;

internal sealed class GetUserHandler(
    IQueryUserRepository repository,
    IQueryLegalRepository legalRepository,
    ILogger<GetUserHandler> logger) : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserWithConsentsAndLegal(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogError("[User: {userId}] User not found", request.UserId);
            return Result<UserDto>.Failure(GetUserErrors.UserNotFound);
        }

        var privacyPolicy = await legalRepository.GetLatestPrivacyPolicy(cancellationToken);

        var termsAndConditions = await legalRepository.GetLatestTermsAndConditions(cancellationToken);

        var lastConsent = user.UserConsents
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();

        var userDto = new UserDto
        {
            User = user,
            HasActiveConsent = lastConsent?.TermsAndConditionsId == termsAndConditions.Id && lastConsent?.PrivacyPolicyId == privacyPolicy.Id
        };

        logger.LogInformation("[User: {userId}] User retrieved successfully", request.UserId);
        return Result<UserDto>.Success(userDto);
    }
}
