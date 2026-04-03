# Nava.Settings

Lightweight runtime settings library for .NET applications.

## Features
- Persist settings in SQLite
- Strongly-typed settings (generic API)
- Runtime updates with events
- Simple DI integration
- Safe JSON serialization with fallback

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

### 2. Register services
```csharp
builder.Services.AddSettingsWithSqlite(_ => "Data Source=app.db");
builder.Services.AddRuntimeSettings<DemoSettings>();
```

### 3. Initialize
```csharp
await app.Services.InitializeApplicationSettingsAsync();
```

### 4. Use in code
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

### 5. Update settings
```csharp
await provider.UpdateAsync(new DemoSettings
{
    Message = "New value"
});
```

### 6. Subscribe to changes
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