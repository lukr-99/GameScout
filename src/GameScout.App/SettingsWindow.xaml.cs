using System.Windows;
using GameScout.App.Services;
using GameScout.App.ViewModels;

namespace GameScout.App;

/// <summary>Modal editor for persisted GameScout settings.</summary>
public partial class SettingsWindow : Window
{
    private readonly ThemeManager _theme;
    private readonly SettingsViewModel _viewModel;
    private WindowChromeThemer? _themer;

    /// <summary>Initializes the window with its view-model and theme manager.</summary>
    public SettingsWindow(SettingsViewModel viewModel, ThemeManager theme)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        InitializeComponent();
        DataContext = viewModel;
        _theme.ThemeChanged += OnThemeChanged;
        Closed += OnClosed;
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

    private void OnClosed(object? sender, EventArgs e)
        => _theme.ThemeChanged -= OnThemeChanged;

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.Save())
            DialogResult = true;
    }
}
