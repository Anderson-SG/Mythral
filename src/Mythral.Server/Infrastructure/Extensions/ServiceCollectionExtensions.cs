using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Mythral.Server.Infrastructure.Options;
using Mythral.Server.Infrastructure.Database;

namespace Mythral.Server.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        return services;
    }

    public static IServiceCollection AddAppDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Usa Options para obter valores tipados. A configuração já foi vinculada em AddAppOptions.
        services.AddDbContextPool<AppDbContext>((sp, options) =>
        {
            var database = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connectionString = $"Host={database.Host};Port={database.Port};Database={database.Database};Username={database.Username};Password={database.Password};";
            options.UseNpgsql(connectionString, o =>
            {
                o.SetPostgresVersion(16, 0);
            });
        });
        return services;
    }

    // Futuros agrupamentos (mantidos para clareza / expansão):
    // public static IServiceCollection AddCore(this IServiceCollection services) { ... }
    // public static IServiceCollection AddNetworking(this IServiceCollection services) { ... }
    // public static IServiceCollection AddGameLoop(this IServiceCollection services) { ... }
    // public static IServiceCollection AddPersistence(this IServiceCollection services) { ... }
}
