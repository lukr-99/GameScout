using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using FreeGameScout.App.Services;
using FreeGameScout.Core.Aggregation;
using FreeGameScout.Core.Games;
using FreeGameScout.Core.Mvvm;
using MessageBox = System.Windows.MessageBox;

namespace FreeGameScout.App.ViewModels;

/// <summary>
/// Drives the main window: runs a scan, exposes the currently-free and upcoming lists, and wires the
/// refresh / open-in-browser / run-at-startup / theme commands.
/// </summary>
public sealed class MainWindowViewModel : ObservableObject
{
    private readonly FreeGameAggregator _aggregator;
    private readonly StartupRegistration _startup;
    private readonly ThemeManager _theme;

    private bool _isBusy;
    private string _statusText = "Ready.";
    private string? _errorText;
    private string _summaryText = "FreeGameScout";
    private string _lastUpdatedText = string.Empty;
    private bool _isDark;

    /// <summary>Initializes a new <see cref="MainWindowViewModel"/>.</summary>
    public MainWindowViewModel(FreeGameAggregator aggregator, StartupRegistration startup, ThemeManager theme)
    {
        _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        _startup = startup ?? throw new ArgumentNullException(nameof(startup));
        _theme = theme ?? throw new ArgumentNullException(nameof(theme));
        _isDark = theme.Current == AppTheme.Dark;

        RefreshCommand = new RelayCommand(_ => _ = RefreshAsync(), _ => !IsBusy);
        OpenGameCommand = new RelayCommand(OpenGame);
        ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
    }

    /// <summary>Raised on the UI thread after each completed scan, for the tray balloon.</summary>
    public event Action<FreeGameReport>? ScanCompleted;

    /// <summary>Games claimable right now.</summary>
    public ObservableCollection<FreeGame> CurrentlyFree { get; } = [];

    /// <summary>Games announced to become free soon.</summary>
    public ObservableCollection<FreeGame> Upcoming { get; } = [];

    /// <summary>Refreshes the lists from all sources.</summary>
    public RelayCommand RefreshCommand { get; }

    /// <summary>Opens a game's store/claim page in the default browser.</summary>
    public RelayCommand OpenGameCommand { get; }

    /// <summary>Switches between light and dark mode.</summary>
    public RelayCommand ToggleThemeCommand { get; }

    /// <summary>Whether a scan is in progress.</summary>
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsIdle));
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>Convenience inverse of <see cref="IsBusy"/> for binding spinners/empty states.</summary>
    public bool IsIdle => !IsBusy;

    /// <summary>Short status line shown under the header.</summary>
    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    /// <summary>One-line headline summary (also used for the tray balloon).</summary>
    public string SummaryText
    {
        get => _summaryText;
        private set => SetProperty(ref _summaryText, value);
    }

    /// <summary>"Updated ..." caption, empty until the first scan finishes.</summary>
    public string LastUpdatedText
    {
        get => _lastUpdatedText;
        private set => SetProperty(ref _lastUpdatedText, value);
    }

    /// <summary>Per-source error summary, or null when everything succeeded.</summary>
    public string? ErrorText
    {
        get => _errorText;
        private set
        {
            if (SetProperty(ref _errorText, value))
                OnPropertyChanged(nameof(HasError));
        }
    }

    /// <summary>Whether <see cref="ErrorText"/> has content.</summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorText);

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

    /// <summary>Whether dark mode is active (bound to the theme toggle).</summary>
    public bool IsDark
    {
        get => _isDark;
        private set => SetProperty(ref _isDark, value);
    }

    /// <summary>Runs a scan and publishes the results onto the bound collections.</summary>
    public async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        StatusText = "Scanning Epic Games Store & Steam…";
        ErrorText = null;

        try
        {
            FreeGameReport report = await _aggregator.ScanAsync().ConfigureAwait(true);
            Apply(report);
            ScanCompleted?.Invoke(report);
        }
        catch (Exception ex)
        {
            StatusText = "Scan failed.";
            ErrorText = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void Apply(FreeGameReport report)
    {
        CurrentlyFree.Clear();
        foreach (FreeGame game in report.CurrentlyFree)
            CurrentlyFree.Add(game);

        Upcoming.Clear();
        foreach (FreeGame game in report.Upcoming)
            Upcoming.Add(game);

        int free = CurrentlyFree.Count;
        SummaryText = free switch
        {
            0 => "No free games to grab right now.",
            1 => "1 free game to grab right now.",
            _ => $"{free} free games to grab right now.",
        };

        StatusText = Upcoming.Count > 0
            ? $"{Upcoming.Count} more coming soon."
            : "All caught up.";

        LastUpdatedText = $"Updated {report.GeneratedUtc.ToLocalTime():t}";
        ErrorText = report.Errors.Count > 0 ? string.Join(Environment.NewLine, report.Errors) : null;
    }

    private void ToggleTheme()
    {
        AppTheme applied = _theme.Toggle();
        IsDark = applied == AppTheme.Dark;
    }

    private static void OpenGame(object? parameter)
    {
        if (parameter is not FreeGame { Url: { Length: > 0 } url })
            return;

        try
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Couldn't open the link:\n{ex.Message}", "FreeGameScout",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
