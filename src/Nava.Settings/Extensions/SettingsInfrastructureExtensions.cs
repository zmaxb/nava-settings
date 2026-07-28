using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nava.Settings.Abstractions;
using Nava.Settings.Infrastructure;

namespace Nava.Settings.Extensions;

public static class SettingsInfrastructureExtensions
{
    public static IServiceCollection AddSettingsWithSqlite(
        this IServiceCollection services,
        Func<IServiceProvider, string> connectionFactory)
    {
        services.AddLogging();

        services.AddDbContext<ConfigurationDbContext>((sp, options) =>
        {
            var connection = connectionFactory(sp);
            options.UseSqlite(connection);
        });

        services.AddSingleton(_ => SettingsJsonOptions.CreateDefault());

        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<ISettingsStore, JsonSettingsStore>();

        return services;
    }
}
