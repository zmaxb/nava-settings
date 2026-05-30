using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nava.Settings.Abstractions;

namespace Nava.Settings.Infrastructure;

public class RuntimeSettingsProvider<T>(
    IOptions<T> options,
    IServiceScopeFactory scopeFactory,
    ILogger<RuntimeSettingsProvider<T>> logger)
    : ISettingsProvider<T>
    where T : class
{
    private readonly Lock _lock = new();
    private T _settings = options.Value;
    public event Func<T, Task>? SettingsChanged;

    public T Settings
    {
        get
        {
            lock (_lock)
            {
                return _settings;
            }
        }
    }

    public async Task InitializeAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<ISettingsStore>();
        var savedSettings = await store.GetAsync<T>();
        T settings;

        if (savedSettings is not null)
        {
            logger.LogInformation("Loaded settings from storage for {Type}", typeof(T).Name);

            lock (_lock)
            {
                _settings = savedSettings;
                settings = _settings;
            }
        }
        else
        {
            logger.LogWarning("Using default settings for {Type}", typeof(T).Name);

            lock (_lock)
            {
                settings = _settings;
            }
        }

        await NotifySettingsChangedAsync(settings);
    }

    public async Task UpdateAsync(T settings)
    {
        using var scope = scopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<ISettingsStore>();
        await store.SaveAsync(settings);

        lock (_lock)
        {
            _settings = settings;
        }

        await NotifySettingsChangedAsync(settings);
    }

    private async Task NotifySettingsChangedAsync(T settings)
    {
        Func<T, Task>? handler;

        lock (_lock)
        {
            handler = SettingsChanged;
        }

        if (handler is null)
        {
            return;
        }

        foreach (var subscription in handler.GetInvocationList().Cast<Func<T, Task>>())
        {
            await subscription(settings);
        }
    }
}
