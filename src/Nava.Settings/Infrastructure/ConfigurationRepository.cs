using Microsoft.EntityFrameworkCore;
using Nava.Settings.Abstractions;

namespace Nava.Settings.Infrastructure;

public sealed class ConfigurationRepository(ConfigurationDbContext db) : IConfigurationRepository
{
    public async Task<string?> GetAsync(string key)
    {
        return await db.Configurations
            .AsNoTracking()
            .Where(x => x.Key == key)
            .Select(x => x.Value)
            .FirstOrDefaultAsync();
    }

    public async Task SetAsync(string key, string value)
    {
        // Atomic UPSERT (SQLite/PostgreSQL) to guarantee safe concurrent writes
        // and avoid duplicate key errors under race conditions
        await db.Database.ExecuteSqlInterpolatedAsync(
            $"""

             INSERT INTO ConfigurationEntries (Key, Value)
             VALUES ({key}, {value})
             ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
                     
             """);
    }
}