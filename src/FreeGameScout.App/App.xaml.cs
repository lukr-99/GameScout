using System.Drawing;
using System.Net.Http;
using System.Windows;
using System.Windows.Forms;
using FreeGameScout.App.Services;
using FreeGameScout.App.ViewModels;
using FreeGameScout.Core.Aggregation;
using FreeGameScout.Core.Games;
using FreeGameScout.Core.Net;
using FreeGameScout.Core.Sources.Epic;
using FreeGameScout.Core.Sources.GamerPower;
using Application = System.Windows.Application;

namespace FreeGameScout.App;

/// <summary>
/// Application composition root. Wires the Core services by hand (no DI container needed for an app
/// this small), owns the tray icon, and runs the initial scan on launch.
/// </summary>
public partial class App : Application, IDisposable
{
    private HttpClient? _http;
    private NotifyIcon? _tray;
    private MainWindow? _window;
    private MainWindowViewModel? _viewModel;
    private ScanLog? _log;
    private bool _exiting;

    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        bool startInTray = e.Args.Any(a =>
            a.Equals(StartupRegistration.StartupArgument, StringComparison.OrdinalIgnoreCase));

        _log = new ScanLog();
        _log.Info($"app started ({(startInTray ? "tray" : "window")} mode)");

        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("FreeGameScout/0.1 (+https://github.com/lukr-99)");

        var httpText = new HttpTextClient(_http);
        var aggregator = new FreeGameAggregator(
        [
            new EpicFreeGamesSource(httpText),
            new GamerPowerSource(httpText),
        ]);

        var startup = new StartupRegistration();
        var theme = new ThemeManager(this);

        _viewModel = new MainWindowViewModel(aggregator, startup, theme);
        _viewModel.ScanCompleted += OnScanCompleted;

        SetupTrayIcon();

        _window = new MainWindow { DataContext = _viewModel };
        if (!startInTray)
            _window.Show();

        // Kick off the first scan immediately so the rundown is ready by the time the user looks.
        _ = _viewModel.RefreshAsync();
    }

    /// <summary>Brings the main window to the foreground, restoring it from the tray if needed.</summary>
    public void ShowWindow()
    {
        if (_window is null)
            return;

        _window.Show();
        _window.WindowState = WindowState.Normal;
        _window.Activate();
        _window.Topmost = true;
        _window.Topmost = false;
    }

    /// <summary>Hides the main window to the notification area without exiting.</summary>
    public void HideToTray() => _window?.Hide();

    /// <summary>Fully exits the application, removing the tray icon.</summary>
    public void ExitApp()
    {
        _exiting = true;
        if (_tray is not null)
        {
            _tray.Visible = false;
            _tray.Dispose();
            _tray = null;
        }

        Shutdown();
    }

    /// <summary>Whether an explicit exit is in progress (used by the window to allow closing).</summary>
    public bool IsExiting => _exiting;

    /// <inheritdoc/>
    protected override void OnExit(ExitEventArgs e)
    {
        Dispose();
        base.OnExit(e);
    }

    /// <summary>Disposes the tray icon and HTTP client.</summary>
    public void Dispose()
    {
        _tray?.Dispose();
        _tray = null;
        _http?.Dispose();
        _http = null;
        GC.SuppressFinalize(this);
    }

    private void SetupTrayIcon()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Show rundown", null, (_, _) => ShowWindow());
        menu.Items.Add("Refresh now", null, (_, _) => _ = _viewModel?.RefreshAsync());
        menu.Items.Add(new ToolStripSeparator());

        var startupItem = new ToolStripMenuItem("Run at Windows startup")
        {
            CheckOnClick = true,
            Checked = _viewModel?.RunAtStartup ?? false,
        };
        startupItem.CheckedChanged += (_, _) =>
        {
            if (_viewModel is not null)
                _viewModel.RunAtStartup = startupItem.Checked;
        };
        menu.Items.Add(startupItem);

        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitApp());

        _tray = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "FreeGameScout",
            Visible = true,
            ContextMenuStrip = menu,
        };
        _tray.DoubleClick += (_, _) => ShowWindow();
    }

    private void OnScanCompleted(FreeGameReport report)
    {
        _log?.Record(report);

        if (_tray is null)
            return;

        // Only surface a balloon when the window isn't already showing the result.
        bool windowVisible = _window is { IsVisible: true, WindowState: not WindowState.Minimized };
        if (windowVisible)
            return;

        int free = report.CurrentlyFree.Count();
        string title = free > 0 ? $"{free} free game{(free == 1 ? string.Empty : "s")} available" : "No free games right now";
        string body = free > 0
            ? string.Join(", ", report.CurrentlyFree.Take(4).Select(g => g.Title))
            : "Nothing to grab at the moment.";

        _tray.BalloonTipTitle = title;
        _tray.BalloonTipText = body;
        _tray.ShowBalloonTip(6000);
    }
}
