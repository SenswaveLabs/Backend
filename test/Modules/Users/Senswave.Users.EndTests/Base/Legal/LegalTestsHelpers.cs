namespace Senswave.Users.EndTests.Base.Legal;

internal static class LegalTestsHelpers
{
    public static string GetLatestVersion(string dirPath)
    {
        if (!Directory.Exists(dirPath))
            throw new InvalidOperationException("Missing directory.");

        var versionDirs = Directory.GetDirectories(dirPath);

        if (versionDirs.Length == 0)
            throw new InvalidOperationException($"{dirPath} does not exist");

        var latestVersionDir = versionDirs
            .Select(d => new
            {
                Path = d,
                Version = Version.Parse(new DirectoryInfo(d).Name)
            })
            .Where(x => x.Version != null)
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        return latestVersionDir!.Version.ToString();
    }
}