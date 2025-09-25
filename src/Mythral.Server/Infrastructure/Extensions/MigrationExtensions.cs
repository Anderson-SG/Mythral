using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Mythral.Server.Infrastructure.Database; // AppDbContext
using System.Linq; // Any()

namespace Mythral.Server.Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pending = await db.Database.GetPendingMigrationsAsync();
        if (pending.Any())
        {
            Log.Information("Aplicando {Count} migrations pendentes...", pending.Count());
            await db.Database.MigrateAsync();
            Log.Information("Migrations aplicadas com sucesso.");
        }
        else
        {
            Log.Information("Nenhuma migration pendente encontrada.");
        }
    }
}
