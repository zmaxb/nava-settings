using Nava.Settings;
using Nava.Settings.Abstractions;
using Nava.Settings.DependencyInjection;
using Nava.Settings.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSettingsWithSqlite(_ => "Data Source=demo.db");
builder.Services.AddRuntimeSettings<DemoSettings>();

var app = builder.Build();

await app.Services.InitializeApplicationSettingsAsync();

var provider = app.Services.GetRequiredService<ISettingsProvider<DemoSettings>>();
provider.Subscribe(s => { Console.WriteLine($"SUBSCRIBE: {s.Message} | {s.Theme}"); });

app.MapGet("/", (ISettingsProvider<DemoSettings> provider) =>
{
    var s = provider.Settings;

    var html = $"""
                <html>
                <body style="font-family:sans-serif; background:{(s.Theme == "dark" ? "#111;" : "#f5f5f5;")}">
                    
                    <div style="max-width:500px;margin:50px auto;padding:20px;border-radius:10px;background:#fff;box-shadow:0 10px 30px rgba(0,0,0,0.1)">
                        
                        <h2>Runtime Settings Demo</h2>
                            
                        <h3>Saved values:</h3>
                        <p><b>Message:</b> {s.Message}</p>
                        <p><b>Theme:</b> {s.Theme}</p>

                        <hr/>

                        <form method="post" action="/update" style="display:flex;flex-direction:column;gap:10px;margin-top:10px">
                            <label>Message</label>
                            <input name="message" value="{s.Message}" placeholder="Message"/>

                            <label>Theme</label>
                            <select name="theme">
                                <option value="light" {(s.Theme == "light" ? "selected" : "")}>Light</option>
                                <option value="dark" {(s.Theme == "dark" ? "selected" : "")}>Dark</option>
                            </select>

                            <button style="padding:10px;border:none;border-radius:6px;background:#007bff;color:white">
                                Save
                            </button>

                        </form>

                    </div>

                </body>
                </html>
                """;

    return Results.Content(html, "text/html");
});

app.MapPost("/update", async (HttpRequest req, ISettingsProvider<DemoSettings> provider) =>
{
    var form = await req.ReadFormAsync();

    var message = form["message"].ToString();
    var theme = form["theme"].ToString();

    await provider.UpdateAsync(new DemoSettings
    {
        Message = string.IsNullOrWhiteSpace(message) ? "Hello world" : message,
        Theme = theme is "dark" ? "dark" : "light"
    });

    return Results.Redirect("/");
});

app.Run();

[SettingsKey("demo")]
public class DemoSettings
{
    public string Message { get; set; } = "Hello world";
    public string Theme { get; set; } = "light";
}