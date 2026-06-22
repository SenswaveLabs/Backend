using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Senswave.Infrastructure.Persistence;

namespace Senswave.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    public static DbContextOptions<T> GetPostgresOptions<T>(this IOptions<PostgreSqlOptions> options)
        where T : DbContext
    {
        var connectionString = options.Value.ConnectionString;

        var builder = new DbContextOptionsBuilder<T>();
        builder.UseNpgsql(connectionString);

        return builder.Options;
    }
}
