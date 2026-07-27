namespace Nava.Settings.Abstractions;

public interface ISettingsStore
{
    Task<T?> GetAsync<T>(
        string? scope = null)
        where T : class;

    Task SaveAsync<T>(
        T settings,
        string? scope = null)
        where T : class;

    Task RemoveAsync<T>(
        string? scope = null)
        where T : class;
}