using Microsoft.Extensions.DependencyInjection;
using Nava.Settings.Abstractions;

namespace Nava.Settings.Infrastructure;

public class ScopedSettingsProvider<T>(
    IServiceScopeFactory scopeFactory)
    : IScopedSettingsProvider<T>
    where T : class
{
    public async Task<T?> GetAsync(string scopeId)
    {
        using var scope = scopeFactory.CreateScope();

        var store = scope.ServiceProvider
            .GetRequiredService<ISettingsStore>();

        return await store.GetAsync<T>(scopeId);
    }

    public async Task UpdateAsync(T settings, string scopeId)
    {
        using var scope = scopeFactory.CreateScope();

        var store = scope.ServiceProvider
            .GetRequiredService<ISettingsStore>();

        await store.SaveAsync(settings, scopeId);
    }

    public async Task RemoveAsync(string scopeId)
    {
        using var scope = scopeFactory.CreateScope();

        var store = scope.ServiceProvider
            .GetRequiredService<ISettingsStore>();

        await store.RemoveAsync<T>(scopeId);
    }
}