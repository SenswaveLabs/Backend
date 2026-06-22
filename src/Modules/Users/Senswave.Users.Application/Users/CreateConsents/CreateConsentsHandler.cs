using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Repositories;

namespace Senswave.Users.Application.Users.CreateConsents;

internal sealed class CreateConsentsHandler(
    ICommandUserRepository commandUserRepository,
    IQueryLegalRepository queryLegalRepository,
    ILogger<CreateConsentsCommand> logger) : ICommandHandler<CreateConsentsCommand>
{
    public async Task<Result> Handle(CreateConsentsCommand request, CancellationToken cancellationToken)
    {
        var user = await commandUserRepository.GetUserWithConsents(request.UserId, cancellationToken);

        if (user is null)
        {
            logger.LogCritical("[User: {userId}] Couldn't find user", request.UserId);
            return Result.Failure(CreateConsentsErrors.UserNotFound);
        }

        var terms = await queryLegalRepository.GetLatestTermsAndConditions(cancellationToken);

        var privacy = await queryLegalRepository.GetLatestPrivacyPolicy(cancellationToken);

        var lastConsent = user.UserConsents
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();

        if (lastConsent is not null &&
            lastConsent.TermsAndConditionsId == terms.Id &&
            lastConsent.PrivacyPolicyId == privacy.Id)
        {
            logger.LogInformation("[User: {userId}] User has already accepted the conditions.", request.UserId);
            return Result.Success();
        }

        var consent = new UserConsents
        {
            UserId = user.Id,
            TermsAndConditionsId = terms.Id,
            PrivacyPolicyId = privacy.Id,
            CreatedAtUtc = DateTime.UtcNow,
        };

        var update = await commandUserRepository.CreateUserConsents(consent, cancellationToken);

        if (update.IsFailure)
        {
            logger.LogError("[User: {userId}] Failed to update user consents.", request.UserId);
            return Result.Failure(CreateConsentsErrors.FailedToAcceptConditions);
        }

        logger.LogInformation("[User: {userId}] Consents given to user.", request.UserId);
        return Result.Success();
    }
}
