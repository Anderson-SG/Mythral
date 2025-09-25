using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mythral.Server.Application.Utils;
using Mythral.Server.Infrastructure.Extensions;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog; // (mantido caso queira reverter futuramente)

ServerUtils.PrintServerLogo();

const string serviceName = "Mythral.Server";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        // Remove provedores padrão e configura Console + OpenTelemetry
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddOpenTelemetry(o =>
        {
            o.IncludeScopes = true;
            o.IncludeFormattedMessage = true;
            o.ParseStateValues = true;
            o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));

            o.AddConsoleExporter();
            o.AddOtlpExporter();
        });
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService(serviceName: serviceName))
            .WithTracing(builder => builder
                // Adicione instrumentações extras conforme necessário
                .AddConsoleExporter()
                .AddOtlpExporter(options =>
                {
                    options.Protocol = OtlpExportProtocol.Grpc;
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(ops => ops.SetDbStatementForText = true)
                .AddRedisInstrumentation()
             )
            .WithMetrics(builder => builder
                // Adicione instrumentações extras conforme necessário
                .AddConsoleExporter()
                .AddOtlpExporter(options =>
                {
                    options.Protocol = OtlpExportProtocol.Grpc;
                })
                .AddHttpClientInstrumentation()
             );

        services
            .AddAppOptions(ctx.Configuration)
            .AddAppDatabase(ctx.Configuration)
            .AddRedisCache(ctx.Configuration)
            .AddGameLoop();
    })
    .Build();

try
{
    await host.Services.ApplyDatabaseMigrationsAsync();
}
catch (Exception ex)
{
    // Log crítico ainda será enviado via ILogger<T>/ILoggerFactory
    Log.Fatal(ex, "Falha ao aplicar migrations.");
    throw; // Fail fast se as migrations não puderem ser aplicadas
}

await host.RunAsync();