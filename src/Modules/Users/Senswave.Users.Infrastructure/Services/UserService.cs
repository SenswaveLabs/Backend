using Microsoft.AspNetCore.Identity;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Enums;
using Senswave.Users.Domain.Extensions;
using Senswave.Users.Domain.Interfaces;
using Senswave.Users.Domain.Repositories;
using Senswave.Users.Infrastructure.Errors;
using Senswave.Users.Infrastructure.Options;

namespace Senswave.Users.Infrastructure.Services;

internal sealed class UserService(
    IOptionsSnapshot<UserServiceOptions> options,
    IQueryUserRepository queryUserRepository,
    ILegalService legalService,
    UsersContext context,
    UserManager<User> userManager,
    ILogger<UserService> logger) : IUserService
{
    private readonly Dictionary<Guid, DateTime> _userConsentCache = [];

    public async Task<Result> UserHasLatestConsents(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!options.Value.ForceConsentsInRquests)
        {
            logger.LogDebug("[UserId: {UserId}] User consent check bypassed as per configuration.", userId);
            return Result.Success();
        }

        if (_userConsentCache.TryGetValue(userId, out var cachedTime))
        {
            var cacheDuration = TimeSpan.FromSeconds(options.Value.ConsentCacheDurationInSeconds);

            if (DateTime.UtcNow - cachedTime < cacheDuration)
            {
                logger.LogDebug("[UserId: {UserId}] User consent check served from cache.", userId);
                return Result.Success();
            }
        }

        var privacy = await legalService.GetPrivacyPolicy(cancellationToken);

        if (privacy.IsFailure)
            return Result.Failure(privacy.Errors);

        var terms = await legalService.GetTermsAndConditions(cancellationToken);

        if (terms.IsFailure)
            return Result.Failure(terms.Errors);

        var user = await queryUserRepository.GetUserWithConsentsAndLegal(userId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("[UserId: {UserId}] User not found when checking consents.", userId);
            return Result.Failure(Error.ServerError("UserNotFound", "User not found."));
        }

        var lastConsent = user.UserConsents
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefault();

        var hasConsented = lastConsent?.TermsAndConditionsId == terms.Data.Id && lastConsent?.PrivacyPolicyId == privacy.Data.Id;

        if (!hasConsented)
        {
            logger.LogWarning("[UserId: {UserId}] User does not have latest consents.", userId);
            return Result.Failure(Error.Failure("NoConsents", "Please consent to latest privacy policy and terms and conditions first."));
        }

        _userConsentCache[userId] = DateTime.UtcNow;
        logger.LogDebug("[UserId: {UserId}] User has latest consents.", userId);

        return Result.Success();
    }

    public async Task<Result<User>> Register(ExternalProvider provider, string email, string subject, CancellationToken cancellationToken = default)
    {
        var providerName = provider.FromExternalProvider();
        var userLoginInfo = new UserLoginInfo(providerName, subject, providerName);

        logger.LogDebug("[Provider: {provider}] [Subject: {subject}] Starting register of external user.",
            providerName, subject);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = new User
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user);

            if (result.Errors.Any())
            {
                logger.LogError("[Subject: {subject}]Failed to create google user. {result}",
                    subject, result.ToString());
                await transaction.RollbackAsync(cancellationToken);
                return Result<User>.Failure(ExternalUserServiceErrors.FailedToCreateUser);
            }

            var userLoginInfoResult = await userManager.AddLoginAsync(user, userLoginInfo);

            if (userLoginInfoResult.Errors.Any())
            {
                logger.LogError("[Subject: {subject}]Failed to add login info to google user. {result}",
                    subject, result.ToString());
                await transaction.RollbackAsync(cancellationToken);
                return Result<User>.Failure(ExternalUserServiceErrors.FailedToAttachUserInfo);
            }

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("[UserId: {UserId}] [Subject: {subject}] External user registered successfully.",
                user.Id, subject);

            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Subject: {subject}]An error occurred while registering external user.",
                subject);

            await transaction.RollbackAsync(cancellationToken);

            return Result<User>.Failure(ExternalUserServiceErrors.ExternalUserRegistrationFailed);
        }
    }

    public async Task<Result> LinkExtenralProvider(ExternalProvider provider, User user, string subject, CancellationToken cancellationToken = default)
    {
        var providerName = provider.FromExternalProvider();

        var logins = await userManager.GetLoginsAsync(user);

        if (logins.Any(x => x.LoginProvider == providerName))
        {
            logger.LogInformation("[Subject: {subject}][Provider: {provider}] User already linked with provider.",
                subject, providerName);
            return Result.Success();
        }

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var userLoginInfo = new UserLoginInfo(providerName, subject, providerName);

            var userLoginInfoResult = await userManager
                .AddLoginAsync(user, userLoginInfo);

            if (userLoginInfoResult.Errors.Any())
            {
                logger.LogError("[Subject: {subject}] Failed to add login info to existing user. {result}",
                    subject, userLoginInfoResult.ToString());

                await transaction.RollbackAsync(cancellationToken);
                return Result<User>.Failure(ExternalUserServiceErrors.FailedToAttachUserInfo);
            }

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("[UserId: {UserId}] [Subject: {subject}] External user linked successfully.",
                user.Id, subject);

            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Subject: {subject}] An error occurred while linking user.",
                subject);

            await transaction.RollbackAsync(cancellationToken);

            return Result<User>.Failure(ExternalUserServiceErrors.ExternalUserLinkingFailed);
        }
    }
}
