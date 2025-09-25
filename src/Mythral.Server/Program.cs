using Microsoft.Extensions.Hosting;
using Mythral.Server.Application.Utils;
using Serilog;
using Mythral.Server.Infrastructure.Extensions;

ServerUtils.PrintServerLogo();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services
            .AddAppOptions(ctx.Configuration)
            .AddAppDatabase(ctx.Configuration)
            .AddRedisCache(ctx.Configuration)
            .AddGameLoop();

        // Futuras separações:
        // .AddCore()
        // .AddNetworking()
        // .AddPersistence()
    })
    .UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/mythral-.log", rollingInterval: RollingInterval.Day)
    )
    .Build();

try
{
    await host.Services.ApplyDatabaseMigrationsAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha ao aplicar migrations.");
    throw; // Fail fast se as migrations não puderem ser aplicadas
}

await host.RunAsync();