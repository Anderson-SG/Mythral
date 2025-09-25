using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mythral.Server.Application.GameLoop;
using Mythral.Server.Application.Networking;
using Mythral.Server.Config;
using Mythral.Server.Domain.Game;
using Mythral.Server.Domain.Networking;
using Mythral.Server.Infrastructure.Database;
using Mythral.Server.Infrastructure.Options;
using Mythral.Server.Serialization;

namespace Mythral.Server.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        // Server options (porta TCP/UDP, tick, etc.)
        services.Configure<ServerOptions>(configuration.GetSection("Server"));
        return services;
    }

    public static IServiceCollection AddAppDatabase(this IServiceCollection services, IConfiguration configuration)
    {
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

    public static IServiceCollection AddGameLoop(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSerializer, MessagePackSerializerAdapter>();
        services.AddSingleton<IGameWorld, GameWorld>();
        services.AddSingleton<IUdpServer, UdpServerStub>();
        services.AddSingleton<ITcpServer, TcpServerStub>();
        services.AddHostedService<GameLoopService>();
        return services;
    }
}
