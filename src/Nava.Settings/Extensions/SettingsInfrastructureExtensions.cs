using System.Text.Json;
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
        services.AddDbContext<ConfigurationDbContext>((sp, options) =>
        {
            var connection = connectionFactory(sp);
            options.UseSqlite(connection);
        });

        services.AddSingleton<JsonSerializerOptions>(_ =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
        services.AddScoped<ISettingsStore, JsonSettingsStore>();

        return services;
    }
}