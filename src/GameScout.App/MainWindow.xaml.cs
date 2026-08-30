using System.ComponentModel;
using System.Windows;
using GameScout.App.Services;
using GameScout.App.ViewModels;

namespace GameScout.App;

/// <summary>
/// The shell window. View wiring only: keeps the native chrome tinted to the theme, and the close
/// box hides to the tray (full exit is via the tray menu).
/// </summary>
public partial class MainWindow : Window
{
    private readonly ThemeManager _theme;
    private WindowChromeThemer? _themer;

    /// <summary>Initializes the window with its view-model and theme manager (injected).</summary>
    public MainWindow(MainWindowViewModel viewModel, ThemeManager theme)
    {
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        InitializeComponent();
        // Show the running version in the title bar so a dev/test build is distinct from a release.
        Title = AppInfo.TitleWithVersion;
        DataContext = viewModel;
        _theme.ThemeChanged += OnThemeChanged;
    }

    /// <inheritdoc/>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _themer = new WindowChromeThemer(this);
        ApplyChrome();
    }

    private void OnThemeChanged(object? sender, EventArgs e) => ApplyChrome();

    private void ApplyChrome() => _themer?.Apply(_theme.Current == AppTheme.Dark);

    /// <inheritdoc/>
    protected override void OnClosing(CancelEventArgs e)
    {
        // Unless the app is really exiting, keep it alive in the tray instead of closing.
        if (System.Windows.Application.Current is App { IsExiting: false })
        {
            e.Cancel = true;
            Hide();
        }

        base.OnClosing(e);
    }

    private void OnHideClick(object sender, RoutedEventArgs e)
    {
        if (System.Windows.Application.Current is App app)
            app.HideToTray();
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        if (System.Windows.Application.Current is App app)
            app.ShowSettings(this);
    }
}
