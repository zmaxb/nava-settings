namespace Nava.Settings.Tests;

[SettingsKey("test")]
public class TestSettings
{
    public string Value { get; set; } = string.Empty;
}