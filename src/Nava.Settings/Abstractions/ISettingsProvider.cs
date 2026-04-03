namespace Nava.Settings.Abstractions;

public interface ISettingsProvider<T> : IRuntimeSettingsInitializer
{
    T Settings { get; }
    Task UpdateAsync(T settings);
    public event Action<T>? SettingsChanged;
}