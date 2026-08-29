using GameScout.Core.Settings;

namespace GameScout.App.Services;

/// <summary>Owns the current persisted settings for the application process.</summary>
public sealed class SettingsService
{
    private readonly JsonGameScoutSettingsStore _store;

    /// <summary>Initializes the service and loads the current settings from disk.</summary>
    /// <param name="store">JSON settings store.</param>
    public SettingsService(JsonGameScoutSettingsStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        Current = store.Load();
    }

    /// <summary>The most recently loaded or saved settings.</summary>
    public GameScoutSettings Current { get; private set; }

    /// <summary>Validates and persists <paramref name="settings"/>.</summary>
    /// <param name="settings">Settings to save.</param>
    public void Save(GameScoutSettings settings)
    {
        _store.Save(settings);
        Current = _store.Load();
    }
}
