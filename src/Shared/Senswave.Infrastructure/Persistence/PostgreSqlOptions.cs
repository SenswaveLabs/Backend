namespace Senswave.Infrastructure.Persistence;

public class PostgreSqlOptions
{
    public const string SectionName = "PostgreSql";

    public string Host { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string ErrorDetail { get; set; } = "true";

    public string ConnectionString => $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};Include Error Detail={ErrorDetail};";
}
