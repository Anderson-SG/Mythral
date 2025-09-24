// See https://aka.ms/new-console-template for more information



using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Mythral.Server.Application.Utils;
using Mythral.Server.Infrastructure.Options;
using Mythral.Server.Infrastructure.Database;
using Serilog;

ServerUtils.PrintServerLogo();

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // Configuration bindings (appsettings.json / appsettings.{Environment}.json)
        services.AddOptions();
        services.Configure<DatabaseOptions>(ctx.Configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<RedisOptions>(ctx.Configuration.GetSection(RedisOptions.SectionName));

        // DbContext (PostgreSQL)
        services.AddDbContextPool<AppDbContext>((sp, options) =>
        {
            var database = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connectionString = $"Host={database.Host};Port={database.Port};Database={database.Database};Username={database.Username};Password={database.Password};";
            options.UseNpgsql(connectionString, o =>
            {
                o.SetPostgresVersion(16, 0);
            });
        });

        // // Core
        // services.AddSingleton<ISessionRegistry, SessionRegistry>();
        // services.AddSingleton<IGameWorld, GameWorld>(); // estado/mundo em memória
        // services.AddSingleton<IClock, SystemClock>();

        // // Networking
        // services.AddSingleton<IUdpServer, LiteNetLibServer>();
        // services.AddSingleton<ITcpServer, TcpNetCoreServer>();

        // // Serialization
        // services.AddSingleton<IMessageSerializer, MessagePackSerializerAdapter>();

        // // Loop autoritativo (60 Hz)
        // services.AddHostedService<GameLoopService>();
        // services.AddHostedService<TcpHostedService>();
        // services.AddHostedService<UdpHostedService>();

        // // Persistência
        // services.AddSingleton<IPlayerRepository, PlayerRepository>(); // Dapper/EF Core
    })
    .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs/mythral-.log", rollingInterval: RollingInterval.Day)
                    )
    .RunConsoleAsync();