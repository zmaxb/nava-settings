namespace Nava.Settings;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SettingsKeyAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}