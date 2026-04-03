namespace Nava.Settings.Abstractions;

public interface IRuntimeSettingsInitializer
{
    Task InitializeAsync();
}