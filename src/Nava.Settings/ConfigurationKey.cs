namespace Nava.Settings;

public static class ConfigurationKey
{
    public static string For<T>(string? subKey = null)
    {
        var type = typeof(T);

        var attr = type.GetCustomAttributes(typeof(SettingsKeyAttribute), false)
            .Cast<SettingsKeyAttribute>()
            .FirstOrDefault();

        if (attr == null)
            throw new InvalidOperationException($"The type {type} has no SettingsKeyAttribute");

        return subKey is null
            ? attr.Key
            : $"{attr.Key}:{subKey}";
    }
}