namespace Nava.Settings.Abstractions;

public interface IScopedSettingsProvider<T>
    where T : class
{
    Task<T?> GetAsync(string scopeId);
    Task UpdateAsync(T settings, string scopeId);
    Task RemoveAsync(string scopeId);
}