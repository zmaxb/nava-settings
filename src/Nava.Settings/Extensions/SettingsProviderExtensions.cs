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

    private sealed class Subscription(Action dispose) : IDisposable
    {
        public void Dispose()
        {
            dispose();
        }
    }
}