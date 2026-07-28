using Microsoft.Extensions.DependencyInjection;
using Nava.Settings.Abstractions;
using Nava.Settings.Infrastructure;

namespace Nava.Settings.Extensions;

public static class SettingsServiceCollectionExtensions
{
    public static void AddRuntimeSettings<T>(this IServiceCollection services)
        where T : class
    {
        services.AddOptions<T>();

        services.AddSingleton<RuntimeSettingsProvider<T>>();

        services.AddSingleton<ISettingsProvider<T>>(sp =>
            sp.GetRequiredService<RuntimeSettingsProvider<T>>());

        services.AddSingleton<IRuntimeSettingsInitializer>(sp =>
            sp.GetRequiredService<RuntimeSettingsProvider<T>>());
    }

    public static void AddScopedSettings<T>(this IServiceCollection services)
        where T : class
    {
        services.AddSingleton<ScopedSettingsProvider<T>>();

        services.AddSingleton<IScopedSettingsProvider<T>>(sp =>
            sp.GetRequiredService<ScopedSettingsProvider<T>>());
    }
}
