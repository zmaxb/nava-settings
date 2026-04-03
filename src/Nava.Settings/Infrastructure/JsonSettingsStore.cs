using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nava.Settings.Abstractions;

namespace Nava.Settings.Infrastructure;

public class JsonSettingsStore(
    IConfigurationRepository repository,
    JsonSerializerOptions options,
    ILogger<JsonSettingsStore> logger)
    : ISettingsStore
{
    public async Task<T?> GetAsync<T>() where T : class
    {
        var key = ConfigurationKey.For<T>();

        var raw = await repository.GetAsync(key);

        if (raw is null) return null;

        try
        {
            return JsonSerializer.Deserialize<T>(raw, options);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize {Key}", key);
            return null;
        }
    }

    public async Task SaveAsync<T>(T settings) where T : class
    {
        var key = ConfigurationKey.For<T>();
        var json = JsonSerializer.Serialize(settings, options);

        await repository.SetAsync(key, json);
    }
}