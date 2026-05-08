# Nava.Settings

Lightweight runtime settings library for .NET applications.

## Features
- Persist settings in SQLite
- Strongly-typed settings (generic API)
- Runtime updates with events
- Simple DI integration
- Safe JSON serialization with fallback values

## Installation
Add project reference or include as a library.

## Usage

### 1. Define settings
```csharp
[SettingsKey("demo")]
public class DemoSettings
{
    public string Message { get; set; } = "Hello";
}
```

### 2. Bootstrap settings loading

In some scenarios settings are required before
dependency injection or application services
are initialized.

`BootstrapReader<T>` allows reading persisted
settings directly from SQLite during early
application startup.

```csharp
const string settingsDbPath = "app.db";

var bootstrapSettings =
    new BootstrapReader<DemoSettings>(settingsDbPath)
        .Read(
            "demo",
            () => new DemoSettings
            {
                Message = "Default message"
            });

Console.WriteLine(bootstrapSettings.Message);
```

Typical use cases:
- Early infrastructure configuration
- Theme/application mode selection
- Logging/bootstrap initialization
- Startup settings before runtime initialization

### 3. Register services
```csharp
builder.Services.AddSettingsWithSqlite(_ => "Data Source=app.db");
builder.Services.AddRuntimeSettings<DemoSettings>();
```

### 4. Initialize
```csharp
await app.Services.InitializeApplicationSettingsAsync();
```

### 5. Use in code
```csharp
public class MyService
{
    private readonly ISettingsProvider<DemoSettings> _provider;

    public MyService(ISettingsProvider<DemoSettings> provider)
    {
        _provider = provider;
    }

    public void DoWork()
    {
        var value = _provider.Settings.Message;
    }
}
```

### 6. Update settings
```csharp
await provider.UpdateAsync(new DemoSettings
{
    Message = "New value"
});
```

### 7. Subscribe to changes
```csharp
provider.Subscribe(s =>
{
    Console.WriteLine($"Updated: {s.Message}");
});
```

## Notes
- Settings are cached in memory
- Updates are persisted and propagated via events
- Each settings type requires `[SettingsKey]`

## License
MIT