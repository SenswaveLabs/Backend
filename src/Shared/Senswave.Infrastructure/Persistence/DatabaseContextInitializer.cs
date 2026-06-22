using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Senswave.Abstractions.Persistence;

namespace Senswave.Infrastructure.Persistence;

public class DatabaseContextInitializer(IServiceProvider serviceProvider, ILogger<DatabaseContextInitializer> logger)
{
    public async Task Initialize(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Migrating database...");

        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(DbContext).IsAssignableFrom(x) && !x.IsInterface && x != typeof(DbContext));

        using var scope = serviceProvider.CreateScope();
        foreach (var dbContextType in dbContextTypes)
        {
            if (scope.ServiceProvider.GetService(dbContextType) is not DbContext dbContext)
                continue;

            logger.LogInformation("Running migration for db context: {context}", dbContextType);
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogDebug("Finished running migrations for db context: {context}", dbContextType);
        }

        var initializers = scope.ServiceProvider
            .GetServices<IDatabaseInitializer>()
            .ToList();

        logger.LogInformation("Initializing database...");

        foreach (var initializer in initializers)
        {
            try
            {
                logger.LogInformation("Running the initializer: {initializer}", initializer.GetType().Name);
                await initializer.Initialize();
                logger.LogDebug("Initializer: {initializer} completed its tasks.", initializer.GetType().Name);
            }
            catch (Exception exception)
            {
                logger.LogError("An error occurred while initializing the database: {message}", exception.Message);
            }
        }
    }
}
