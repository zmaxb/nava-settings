using Nava.Settings.Abstractions;

namespace Nava.Settings.Extensions;

public static class SettingsProviderExtensions
{
    public static IDisposable Subscribe<T>(
        this ISettingsProvider<T> provider,
        Action<T> handler)
    {
        Task HandleSettingsChanged(T settings)
        {
            handler(settings);
            return Task.CompletedTask;
        }

        provider.SettingsChanged += HandleSettingsChanged;

        return new Subscription(() =>
            provider.SettingsChanged -= HandleSettingsChanged);
    }

    public static IDisposable Subscribe<T>(
        this ISettingsProvider<T> provider,
        Func<T, Task> handler)
    {
        provider.SettingsChanged += handler;

        return new Subscription(() =>
            provider.SettingsChanged -= handler);
    }
    
    public static IDisposable Subscribe<T>(
        this ISettingsProvider<T> provider,
        Func<T, Task> handler,
        Action<Exception> onError)
    {
        async Task HandleSettingsChanged(T settings)
        {
            try
            {
                await handler(settings);
            }
            catch (Exception exception)
            {
                onError(exception);
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
