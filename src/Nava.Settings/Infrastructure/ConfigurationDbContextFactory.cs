using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nava.Settings.Infrastructure;

public sealed class ConfigurationDbContextFactory
    : IDesignTimeDbContextFactory<ConfigurationDbContext>
{
    public ConfigurationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ConfigurationDbContext>()
            .UseSqlite("Data Source=app.configuration.db")
            .Options;

        return new ConfigurationDbContext(options);
    }
}