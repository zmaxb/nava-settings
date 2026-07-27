using System.Text.Json;

namespace Nava.Settings.Infrastructure;

internal static class SettingsJsonOptions
{
    public static JsonSerializerOptions CreateDefault()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            WriteIndented = false
        };
    }
}