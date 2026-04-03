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
    public event Action<T>? SettingsChanged;

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

        if (savedSettings is not null)
        {
            logger.LogInformation("Loaded settings from storage for {Type}", typeof(T).Name);

            lock (_lock)
            {
                _settings = savedSettings;
            }
        }
        else
        {
            logger.LogWarning("Using default settings for {Type}", typeof(T).Name);
        }

        Action<T>? handler;

        lock (_lock)
        {
            handler = SettingsChanged;
        }

        handler?.Invoke(_settings);
    }

    public async Task UpdateAsync(T settings)
    {
        using var scope = scopeFactory.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<ISettingsStore>();
        await store.SaveAsync(settings);

        Action<T>? handler;

        lock (_lock)
        {
            _settings = settings;
            handler = SettingsChanged;
        }

        handler?.Invoke(settings);
    }
}