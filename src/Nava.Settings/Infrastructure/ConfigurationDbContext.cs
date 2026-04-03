using Microsoft.EntityFrameworkCore;

namespace Nava.Settings.Infrastructure;

public sealed class ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options) : DbContext(options)
{
    public DbSet<ConfigurationEntry> Configurations => Set<ConfigurationEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ConfigurationDbContext).Assembly);
    }
}