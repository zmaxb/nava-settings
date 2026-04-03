using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nava.Settings.Abstractions;
using Nava.Settings.Infrastructure;

namespace Nava.Settings.DependencyInjection;

public static class InitializeApplicationSettings
{
    public static async Task InitializeApplicationSettingsAsync(this IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        await db.Database.MigrateAsync();

        var initializers = scope.ServiceProvider
            .GetServices<IRuntimeSettingsInitializer>();

        foreach (var initializer in initializers)
            await initializer.InitializeAsync();
    }
}