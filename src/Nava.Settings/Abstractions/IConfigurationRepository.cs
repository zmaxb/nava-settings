namespace Nava.Settings.Abstractions;

public interface IConfigurationRepository
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
}