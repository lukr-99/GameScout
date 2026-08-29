using GameScout.App.Services;
using GameScout.Core.Mvvm;

namespace GameScout.App.ViewModels;

/// <summary>
/// Shell view-model: hosts the "Free now" and "On sale" tabs and the app-level commands
/// (refresh both, toggle theme, run-at-startup).
/// </summary>
public sealed class MainWindowViewModel : ObservableObject
{
    private readonly StartupRegistration _startup;
    private readonly ThemeManager _theme;
    private bool _isDark;

    /// <summary>Initializes a new <see cref="MainWindowViewModel"/>.</summary>
    public MainWindowViewModel(
        FreeGamesViewModel free,
        DealsViewModel deals,
        StartupRegistration startup,
        ThemeManager theme)
    {
        Free = free ?? throw new ArgumentNullException(nameof(free));
        Deals = deals ?? throw new ArgumentNullException(nameof(deals));
        _startup = startup ?? throw new ArgumentNullException(nameof(startup));
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
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

    /// <summary>Whether the app is registered to launch at Windows sign-in.</summary>
    public bool RunAtStartup
    {
        get => _startup.IsEnabled();
        set
        {
            if (RunAtStartup == value)
                return;
            _startup.Set(value);
            OnPropertyChanged();
        }
    }

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

    private void ToggleTheme() => IsDark = _theme.Toggle() == AppTheme.Dark;
}
