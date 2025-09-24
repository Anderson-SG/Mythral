namespace Mythral.Server.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5432;
    public string Database { get; init; } = "mythral";
    public string Username { get; init; } = "mythral";
    public string Password { get; init; } = "mythral";

    public string ToConnectionString()
        => $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};";
}
