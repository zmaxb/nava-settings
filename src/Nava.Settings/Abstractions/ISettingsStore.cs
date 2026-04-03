namespace Nava.Settings.Abstractions;

public interface ISettingsStore
{
    Task<T?> GetAsync<T>() where T : class;
    Task SaveAsync<T>(T settings) where T : class;
}