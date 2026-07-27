using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace Nava.Settings.Infrastructure;

public sealed class BootstrapReader<T>(
    string databasePath,
    JsonSerializerOptions? jsonSerializerOptions = null)
    where T : class
{
    private readonly JsonSerializerOptions _jsonOptions =
        jsonSerializerOptions ?? SettingsJsonOptions.CreateDefault();

    public T Read(
        string key,
        Func<T> fallbackFactory)
    {
        if (!File.Exists(databasePath))
            return fallbackFactory();

        using var connection =
            new SqliteConnection($"Data Source={databasePath}");

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT Value
            FROM ConfigurationEntries
            WHERE Key = $key
            """;

        command.Parameters.AddWithValue("$key", key);

        var json = command.ExecuteScalar() as string;

        if (string.IsNullOrWhiteSpace(json))
            return fallbackFactory();

        return JsonSerializer.Deserialize<T>(json, _jsonOptions)
               ?? fallbackFactory();
    }
}