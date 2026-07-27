# Nava.Settings

Lightweight strongly typed settings library for .NET applications with SQLite persistence, runtime updates, and scope-aware storage.

## Features

* Strongly typed settings
* SQLite persistence
* In-memory caching for runtime settings
* Runtime updates with change notifications
* Scope-aware settings for users, tenants, workspaces, and other contexts
* Early startup settings loading
* Simple dependency injection integration

## Installation

Add a reference to the `Nava.Settings` project.

## Configuration

Register the SQLite settings storage:

```csharp
builder.Services.AddSettingsWithSqlite(_ => "Data Source=settings.db");
```

Every settings type must have a unique `SettingsKey`:

```csharp
[SettingsKey("demo")]
public sealed class DemoSettings
{
    public string Message { get; set; } = "Hello";
}
```

## Runtime settings

Runtime settings represent one application-wide instance of a settings type.

They are loaded during application startup, cached in memory, persisted to SQLite, and can notify subscribers when their value changes.

### Register runtime settings

```csharp
builder.Services.AddRuntimeSettings<DemoSettings>();
```

### Initialize runtime settings

After building the application, initialize all registered runtime settings:

```csharp
await app.Services.InitializeApplicationSettingsAsync();
```

This method also applies pending settings database migrations.

### Read settings

Inject `ISettingsProvider<T>`:

```csharp
public sealed class MyService(
    ISettingsProvider<DemoSettings> settingsProvider)
{
    public void DoWork()
    {
        var message = settingsProvider.Settings.Message;

        Console.WriteLine(message);
    }
}
```

### Update settings

```csharp
await settingsProvider.UpdateAsync(
    new DemoSettings
    {
        Message = "Updated message"
    });
```

The new value is persisted to SQLite and propagated to subscribers.

### Subscribe to changes

```csharp
settingsProvider.Subscribe(settings =>
{
    Console.WriteLine($"Updated: {settings.Message}");
});
```

## Scoped settings

Scoped settings allow storing multiple instances of the same settings type, identified by a scope ID.

A scope can represent:

* a user
* a tenant
* a workspace
* an organization
* any other application-defined context

Scoped settings are loaded on demand and are not cached globally.

### Define scoped settings

```csharp
[SettingsKey("user-appearance")]
public sealed class UserAppearanceSettings
{
    public string Theme { get; set; } = "System";

    public string Culture { get; set; } = "en";
}
```

### Register scoped settings

```csharp
builder.Services.AddScopedSettings<UserAppearanceSettings>();
```

Scoped settings do not require runtime initialization.

### Read scoped settings

Inject `IScopedSettingsProvider<T>` and provide the scope ID:

```csharp
public sealed class UserAppearanceService(
    IScopedSettingsProvider<UserAppearanceSettings> settingsProvider)
{
    public async Task<UserAppearanceSettings> GetAsync(
        string userId)
    {
        return await settingsProvider.GetAsync(userId)
            ?? new UserAppearanceSettings();
    }
}
```

### Save scoped settings

```csharp
await settingsProvider.UpdateAsync(
    new UserAppearanceSettings
    {
        Theme = "Dark",
        Culture = "de"
    },
    userId);
```

### Remove scoped settings

```csharp
await settingsProvider.RemoveAsync(userId);
```

After removal, subsequent reads return null. The application can then provide its own default settings.

## Bootstrap settings

Some settings may be required before dependency injection or application services are fully initialized.

`BootstrapReader<T>` reads persisted settings directly from SQLite during early application startup.

```csharp
const string settingsDbPath = "settings.db";

var settings =
    new BootstrapReader<DemoSettings>(settingsDbPath)
        .Read(
            "demo",
            () => new DemoSettings
            {
                Message = "Default message"
            });

Console.WriteLine(settings.Message);
```

Typical use cases include:

* early infrastructure configuration
* logging initialization
* application mode selection
* settings required before the application host starts

## Complete example

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSettingsWithSqlite(
    _ => "Data Source=settings.db");

builder.Services.AddRuntimeSettings<DemoSettings>();
builder.Services.AddScopedSettings<UserAppearanceSettings>();

var app = builder.Build();

await app.Services.InitializeApplicationSettingsAsync();

app.Run();
```

## Storage model

Settings are serialized as JSON and stored by a generated key.

Runtime settings use the settings type key:

```text
demo
```

Scoped settings combine the settings type key with the supplied scope ID:

```text
user-appearance:<scope-id>
```

Each settings type must use a unique `[SettingsKey]` value.

## Notes

* SQLite storage is managed through Entity Framework Core.
* Runtime settings are cached in memory.
* Scoped settings are resolved for each requested scope.
* Runtime updates are persisted before change notifications are raised.
* Missing scoped settings return `null`.
* Runtime settings use their configured default values when no persisted value exists.
* Invalid persisted JSON read through runtime or scoped providers is logged and treated as a missing value.

## License

MIT