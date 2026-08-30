using System.Globalization;
using System.IO;
using GameScout.App.Services;
using GameScout.Core.Mvvm;
using GameScout.Core.Settings;

namespace GameScout.App.ViewModels;

/// <summary>Editable settings state with validation and persistence.</summary>
public sealed class SettingsViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly StartupRegistration _startup;
    private string _locale;
    private string _country;
    private string _minimumWorthText;
    private bool _runAtStartup;
    private string _errorText = string.Empty;

    /// <summary>Initializes the editor from the current persisted settings.</summary>
    /// <param name="settingsService">Settings owner used to load and save values.</param>
    /// <param name="startup">Windows sign-in launch registration.</param>
    public SettingsViewModel(SettingsService settingsService, StartupRegistration startup)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        _startup = startup ?? throw new ArgumentNullException(nameof(startup));
        GameScoutSettings current = settingsService.Current;
        _locale = current.Locale;
        _country = current.Country;
        _minimumWorthText = current.MinimumWorth.ToString(CultureInfo.CurrentCulture);
        _runAtStartup = startup.IsEnabled();
    }

    /// <summary>Storefront locale, such as en-US or cs-CZ.</summary>
    public string Locale
    {
        get => _locale;
        set => SetProperty(ref _locale, value);
    }

    /// <summary>Two-letter storefront country code.</summary>
    public string Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }

    /// <summary>Editable minimum-worth value in the user's number format.</summary>
    public string MinimumWorthText
    {
        get => _minimumWorthText;
        set => SetProperty(ref _minimumWorthText, value);
    }

    /// <summary>Whether GameScout should launch quietly to the tray at Windows sign-in.</summary>
    public bool RunAtStartup
    {
        get => _runAtStartup;
        set => SetProperty(ref _runAtStartup, value);
    }

    /// <summary>Validation or persistence error shown by the settings window.</summary>
    public string ErrorText
    {
        get => _errorText;
        private set => SetProperty(ref _errorText, value);
    }

    /// <summary>Validates and saves the current editor values.</summary>
    /// <returns><see langword="true"/> when the settings were saved.</returns>
    public bool Save()
    {
        ErrorText = string.Empty;
        if (!TryParseMinimumWorth(MinimumWorthText, out decimal minimumWorth))
        {
            ErrorText = "Minimum worth must be a number, such as 2.99.";
            return false;
        }

        if (!GameScoutSettings.TryCreate(
            Locale,
            Country,
            minimumWorth,
            out GameScoutSettings? settings,
            out string? error))
        {
            ErrorText = error;
            return false;
        }

        // TryCreate resets Theme to its default; carry the currently-persisted theme forward so
        // saving region settings never reverts the user's chosen appearance.
        settings = settings with { Theme = _settingsService.Current.Theme };

        try
        {
            _settingsService.Save(settings);
            _startup.Set(RunAtStartup);
            Locale = settings.Locale;
            Country = settings.Country;
            MinimumWorthText = settings.MinimumWorth.ToString(CultureInfo.CurrentCulture);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorText = $"Could not save settings: {ex.Message}";
            return false;
        }
    }

    private static bool TryParseMinimumWorth(string value, out decimal minimumWorth)
    {
        const NumberStyles styles = NumberStyles.AllowDecimalPoint |
                                    NumberStyles.AllowLeadingSign |
                                    NumberStyles.AllowLeadingWhite |
                                    NumberStyles.AllowTrailingWhite;
        return decimal.TryParse(value, styles, CultureInfo.CurrentCulture, out minimumWorth) ||
               decimal.TryParse(value, styles, CultureInfo.InvariantCulture, out minimumWorth);
    }
}
