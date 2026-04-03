using Microsoft.Extensions.DependencyInjection;
using Nava.Settings.Abstractions;
using Nava.Settings.Infrastructure;

namespace Nava.Settings.Extensions;

public static class RuntimeSettingsServiceCollectionExtensions
{
    public static void AddRuntimeSettings<T>(this IServiceCollection services)
        where T : class
    {
        services.AddSingleton<RuntimeSettingsProvider<T>>();

        services.AddSingleton<ISettingsProvider<T>>(sp =>
            sp.GetRequiredService<RuntimeSettingsProvider<T>>());

        services.AddSingleton<IRuntimeSettingsInitializer>(sp =>
            sp.GetRequiredService<RuntimeSettingsProvider<T>>());
    }
}