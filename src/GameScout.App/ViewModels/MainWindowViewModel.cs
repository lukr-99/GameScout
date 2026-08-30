using System.IO;
using GameScout.App.Services;
using GameScout.Core.Mvvm;

namespace GameScout.App.ViewModels;

/// <summary>
/// Shell view-model: hosts the "Free now" and "On sale" tabs and the app-level commands
/// (refresh both, toggle theme). Run-at-startup lives in the settings window.
/// </summary>
public sealed class MainWindowViewModel : ObservableObject
{
    private readonly ThemeManager _theme;
    private readonly SettingsService _settings;
    private bool _isDark;

    /// <summary>Initializes a new <see cref="MainWindowViewModel"/>.</summary>
    public MainWindowViewModel(
        FreeGamesViewModel free,
        DealsViewModel deals,
        ThemeManager theme,
        SettingsService settings)
    {
        Free = free ?? throw new ArgumentNullException(nameof(free));
        Deals = deals ?? throw new ArgumentNullException(nameof(deals));
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _isDark = theme.Current == AppTheme.Dark;

        RefreshAllCommand = new RelayCommand(_ => RefreshAll());
        ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
    }

    /// <summary>The "Free now" tab.</summary>
    public FreeGamesViewModel Free { get; }

    /// <summary>The "On sale" tab.</summary>
    public DealsViewModel Deals { get; }

    /// <summary>Refreshes both tabs.</summary>
    public RelayCommand RefreshAllCommand { get; }

    /// <summary>Switches between light and dark mode.</summary>
    public RelayCommand ToggleThemeCommand { get; }

    /// <summary>Whether dark mode is active.</summary>
    public bool IsDark
    {
        get => _isDark;
        private set => SetProperty(ref _isDark, value);
    }

    /// <summary>Runs both scans (used on launch and by the tray "Refresh" item).</summary>
    public void RefreshAll()
    {
        _ = Free.RefreshAsync();
        _ = Deals.RefreshAsync();
    }

    private void ToggleTheme()
    {
        AppTheme applied = _theme.Toggle();
        IsDark = applied == AppTheme.Dark;
        PersistTheme(applied);
    }

    private void PersistTheme(AppTheme theme)
    {
        try
        {
            _settings.Save(_settings.Current with { Theme = ThemeManager.ToSettingValue(theme) });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Non-fatal: the theme is already applied for this session; it just won't stick.
        }
    }
}
