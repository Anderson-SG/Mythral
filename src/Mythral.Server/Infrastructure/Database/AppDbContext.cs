using Microsoft.EntityFrameworkCore;
using Mythral.Server.Domain.Entities;

namespace Mythral.Server.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts { get; set; }
}