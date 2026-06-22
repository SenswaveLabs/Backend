using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senswave.Users.Domain;
using Senswave.Users.Domain.Diagnostics;
using Senswave.Users.Domain.Enums;
using Senswave.Users.Domain.Interfaces;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Senswave.Users.Infrastructure.Workers;

public class DeleteAccountBackgroundWorker(
    IServiceProvider serviceProvider,
    IUsersActivityProvider activityProvider,
    ILogger<DeleteAccountBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Delete account background worker is starting.");

        await SendEmailsToRemovedUsers(stoppingToken);

        logger.LogInformation("Delete account background worker is stopping.");
    }

    private async Task SendEmailsToRemovedUsers(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UsersContext>();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<UsersOptions>>();

            using (var activity = activityProvider.StartActivity("WORKER /users/removal", ActivityKind.Internal))
            {
                try
                {
                    var hasUsersWithMessageToSend = await context.RemovedUsers
                        .AnyAsync(x => x.Status == UserRemovalStatus.UserDataRemoved, cancellationToken);

                    if (hasUsersWithMessageToSend && options.Value.DeleteAccount.WorkerEnabled)
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<IDeleteAccountService>();

                        await RemoveUsers(context, options.Value, emailService, cancellationToken);
                    }

                    logger.LogDebug("Delete account background worker check finished");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception occurred in delete account background worker.");
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(options.Value.DeleteAccount.DelaySeconds), cancellationToken);
        }
    }

    private async Task RemoveUsers(
        UsersContext context,
        UsersOptions options,
        IDeleteAccountService deleteAccountService,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            var userToRemove = await context.RemovedUsers
                .Where(x => x.Status == UserRemovalStatus.UserDataRemoved)
                .OrderBy(x => x.CreatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            if (userToRemove is null)
                break;

            try
            {
                logger.LogInformation("[User: {UserId}] Starting user account data removal.", userToRemove.Id);

                await deleteAccountService.DeleteAccountAsync(userToRemove.HashedEmail!);

                var salt = RandomNumberGenerator.GetBytes(16);

                var hash = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(userToRemove.HashedEmail!),
                    salt,
                    200_000,
                    HashAlgorithmName.SHA512,
                    32
                );

                userToRemove.HashedEmail = Convert.ToBase64String(hash);
                userToRemove.SecurityStamp = Convert.ToBase64String(salt);
                userToRemove.UpdatedAtUtc = DateTime.UtcNow;
                userToRemove.Status = UserRemovalStatus.Finalized;

                await context.SaveChangesAsync(cancellationToken);

                logger.LogInformation("[User: {UserId}] User account data removal completed and record hashed.", userToRemove.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while removing user account data.");

                if (DateTime.UtcNow - userToRemove.CreatedAtUtc > TimeSpan.FromSeconds(15 * options.DeleteAccount.DelaySeconds))
                {
                    userToRemove.Status = UserRemovalStatus.Error;
                    userToRemove.UpdatedAtUtc = DateTime.UtcNow;
                    await context.SaveChangesAsync(cancellationToken);

                    logger.LogError("[UserToRemoveId: {userToRemoveId}] User to remove state set to error. Manual intervetion required.", userToRemove.Id);
                }
            }
        }
    }
}
