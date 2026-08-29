using System.Text.Json;

namespace GameScout.Core.Settings;

/// <summary>Loads and saves <see cref="GameScoutSettings"/> as a local JSON file.</summary>
public sealed class JsonGameScoutSettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    private readonly string _filePath;

    /// <summary>Initializes a store at <paramref name="filePath"/>.</summary>
    /// <param name="filePath">Absolute or relative settings-file path.</param>
    public JsonGameScoutSettingsStore(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    /// <summary>The JSON file managed by this store.</summary>
    public string FilePath => _filePath;

    /// <summary>
    /// Loads valid settings, falling back to <see cref="GameScoutSettings.Default"/> when the file
    /// is missing, unreadable, malformed, or contains invalid values.
    /// </summary>
    public GameScoutSettings Load()
    {
        try
        {
            if (!File.Exists(_filePath))
                return GameScoutSettings.Default;

            string json = File.ReadAllText(_filePath);
            GameScoutSettings? persisted = JsonSerializer.Deserialize<GameScoutSettings>(json, SerializerOptions);
            return persisted is not null && GameScoutSettings.TryCreate(
                persisted.Locale,
                persisted.Country,
                persisted.MinimumWorth,
                out GameScoutSettings? normalized,
                out _)
                ? normalized
                : GameScoutSettings.Default;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return GameScoutSettings.Default;
        }
    }

    /// <summary>Saves valid settings, replacing any existing file atomically.</summary>
    /// <param name="settings">Settings to persist.</param>
    public void Save(GameScoutSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!GameScoutSettings.TryCreate(
            settings.Locale,
            settings.Country,
            settings.MinimumWorth,
            out GameScoutSettings? normalized,
            out string? error))
        {
            throw new ArgumentException(error, nameof(settings));
        }

        string fullPath = Path.GetFullPath(_filePath);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        string temporaryPath = fullPath + ".tmp";
        try
        {
            string json = JsonSerializer.Serialize(normalized, SerializerOptions);
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, fullPath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }
}
