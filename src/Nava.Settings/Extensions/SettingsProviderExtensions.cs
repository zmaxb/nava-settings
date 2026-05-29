using Nava.Settings.Abstractions;

namespace Nava.Settings.Extensions;

public static class SettingsProviderExtensions
{
    public static IDisposable Subscribe<T>(
        this ISettingsProvider<T> provider,
        Action<T> handler)
    {
        provider.SettingsChanged += handler;

        return new Subscription(() =>
            provider.SettingsChanged -= handler);
    }
    
    public static IDisposable Subscribe<T>(
        this ISettingsProvider<T> provider,
        Func<T, Task> handler,
        Action<Exception>? onError = null)
    {
        async void HandleSettingsChanged(T settings)
        {
            try
            {
                await handler(settings);
            }
            catch (Exception exception)
            {
                onError?.Invoke(exception);
            }
        }

        provider.SettingsChanged += HandleSettingsChanged;

        return new Subscription(() =>
            provider.SettingsChanged -= HandleSettingsChanged);
    }

    private sealed class Subscription(Action dispose) : IDisposable
    {
        public void Dispose()
        {
            dispose();
        }
    }
}