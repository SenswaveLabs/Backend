using Microsoft.EntityFrameworkCore;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Interfaces;

namespace Senswave.Users.Infrastructure.Services;

public sealed class LagalPpAndTcService(
    UsersContext context, ILogger<LagalPpAndTcService> logger) : ILegalService
{
    public static string PrivacyPolicyDirPath = Path.Combine(AppContext.BaseDirectory, "Resources", "PrivacyPolicy");

    public static string TermsAndConditionsDirPath = Path.Combine(AppContext.BaseDirectory, "Resources", "TermsAndConditions");

    private const string MainFileName = "main.md";

    private const string ChangesFileName = "changes.md";

    public async Task<Result<PrivacyPolicy>> GetPrivacyPolicy(CancellationToken cancellationToken)
    {
        var latestDatbaseEntry = await context.PrivacyPolicies
            .OrderByDescending(p => p.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestDatbaseEntry is null)
        {
            logger.LogError("Latest version of terms and conditions not found.");
            return Result<PrivacyPolicy>.Failure(Error.ServerError("PrivacyPolicyVersionMissing", "Privacy policy version is missing or could not be retrieved."));
        }

        logger.LogDebug("[Version: {version}] Privacy policy found.", latestDatbaseEntry.Version);

        return Result<PrivacyPolicy>.Success(latestDatbaseEntry);
    }

    public async Task<Result<TermsAndConditions>> GetTermsAndConditions(CancellationToken cancellationToken)
    {
        var latestDatbaseEntry = await context.TermsAndConditions
            .OrderByDescending(p => p.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestDatbaseEntry is null)
        {
            logger.LogError("Latest version of terms and conditions not found.");
            return Result<TermsAndConditions>.Failure(Error.ServerError("TermsAndCondtionsVersionMissing", "Terms and conditions version is missing or could not be retrieved."));
        }

        logger.LogDebug("[Version: {version}] Terms and condtions found.", latestDatbaseEntry.Version);

        return Result<TermsAndConditions>.Success(latestDatbaseEntry);
    }

    public async Task<bool> VerifyLegal(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Veryfying legal.");

        var privacyPolicyTask = await CheckPrivacyPolicy(cancellationToken);
        var termsAndConditionsTask = await CheckTermsAndConditions(cancellationToken);

        return privacyPolicyTask && termsAndConditionsTask;
    }

    private async Task<bool> CheckPrivacyPolicy(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Veryfying privacy policy status.");

        if (!Directory.Exists(PrivacyPolicyDirPath))
        {
            logger.LogError("Privacy policy directory not found: {Path}", PrivacyPolicyDirPath);
            return false;
        }

        var versionDirs = Directory.GetDirectories(PrivacyPolicyDirPath);
        if (versionDirs.Length == 0)
        {
            logger.LogWarning("No privacy policy versions found in directory.");
            return false;
        }

        var latestVersionDir = versionDirs
            .Select(d => new
            {
                Path = d,
                Version = Version.Parse(new DirectoryInfo(d).Name)
            })
            .Where(x => x.Version != null)
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        if (latestVersionDir == null)
        {
            logger.LogError("No valid privacy policy version directories found.");
            return false;
        }

        var latestDatabasePrivacyPolicy = await context.PrivacyPolicies
            .OrderByDescending(p => p.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestDatabasePrivacyPolicy is not null && Version.Parse(latestDatabasePrivacyPolicy!.Version) >= latestVersionDir.Version)
        {
            logger.LogInformation("Privacy policy up to date.");
            return true;
        }

        var mainFilePath = Path.Combine(latestVersionDir.Path, MainFileName);
        var markdownContent = await File.ReadAllTextAsync(mainFilePath, cancellationToken);

        var changesFilePath = Path.Combine(latestVersionDir.Path, ChangesFileName);
        var changesContent = File.Exists(changesFilePath) ? await File.ReadAllTextAsync(changesFilePath, cancellationToken) : string.Empty;

        var newPolicy = new PrivacyPolicy
        {
            Version = latestVersionDir.Version.ToString(),
            Content = markdownContent,
            Summary = changesContent
        };

        context.PrivacyPolicies.Add(newPolicy);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("[Version: {version}] New privacy policy added to the database.", latestVersionDir.Version.ToString());

        return true;
    }

    private async Task<bool> CheckTermsAndConditions(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Veryfying terms and conditions status.");

        if (!Directory.Exists(TermsAndConditionsDirPath))
        {
            logger.LogError("Terms and Conditions directory not found: {Path}", TermsAndConditionsDirPath);
            return false;
        }

        var versionDirs = Directory.GetDirectories(TermsAndConditionsDirPath);
        if (versionDirs.Length == 0)
        {
            logger.LogWarning("No versions found in directory.");
            return false;
        }

        var latestVersionDir = versionDirs
            .Select(d => new
            {
                Path = d,
                Version = Version.Parse(new DirectoryInfo(d).Name)
            })
            .Where(x => x.Version != null)
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        if (latestVersionDir == null)
        {
            logger.LogError("No valid version directories found.");
            return false;
        }

        var latestDatbaseEntry = await context.TermsAndConditions
            .OrderByDescending(p => p.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestDatbaseEntry is not null && Version.Parse(latestDatbaseEntry!.Version) >= latestVersionDir.Version)
        {
            logger.LogInformation("Terms and Conditions up to date.");
            return true;
        }

        var mainFilePath = Path.Combine(latestVersionDir.Path, MainFileName);
        var markdownContent = await File.ReadAllTextAsync(mainFilePath, cancellationToken);

        var changesFilePath = Path.Combine(latestVersionDir.Path, ChangesFileName);
        var changesContent = File.Exists(changesFilePath) ? await File.ReadAllTextAsync(changesFilePath, cancellationToken) : string.Empty;

        var newPolicy = new TermsAndConditions
        {
            Version = latestVersionDir.Version.ToString(),
            Content = markdownContent,
            Summary = changesContent
        };

        context.TermsAndConditions.Add(newPolicy);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("[Version: {version}] New terms and conditions added to the database.", latestVersionDir.Version.ToString());

        return true;
    }
}
