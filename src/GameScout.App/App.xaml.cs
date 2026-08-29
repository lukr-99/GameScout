using System.Net.Http;
using System.Windows;
using System.Windows.Forms;
using GameScout.App.Services;
using GameScout.App.ViewModels;
using GameScout.Core.Abstractions;
using GameScout.Core.DependencyInjection;
using GameScout.Core.Games;
using GameScout.Core.Net;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;

namespace GameScout.App;

/// <summary>
/// Application composition root. Builds the DI container, owns the tray icon, and runs the initial
/// scans on launch.
/// </summary>
public partial class App : Application, IDisposable
{
    private const string UserAgent = "GameScout/0.2 (+https://github.com/lukr-99/GameScout)";

    private ServiceProvider? _services;
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

        _services = BuildServices();

        _log = _services.GetRequiredService<ScanLog>();
        _log.Info($"app started ({(startInTray ? "tray" : "window")} mode)");

        _viewModel = _services.GetRequiredService<MainWindowViewModel>();
        _viewModel.Free.ScanCompleted += OnFreeScanCompleted;
        _viewModel.Deals.ScanCompleted += report => _log?.RecordDeals(report);

        SetupTrayIcon();

        _window = _services.GetRequiredService<MainWindow>();
        if (!startInTray)
            _window.Show();

        // Kick off both scans immediately so the rundown is ready by the time the user looks.
        _viewModel.RefreshAll();
    }

    private ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(_ =>
        {
            var http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            return http;
        });
        services.AddSingleton<IHttpTextClient>(sp => new HttpTextClient(sp.GetRequiredService<HttpClient>()));
        services.AddGameScoutCore();

        services.AddSingleton(_ => new ThemeManager(this));
        services.AddSingleton<StartupRegistration>();
        services.AddSingleton<ScanLog>();

        services.AddSingleton<FreeGamesViewModel>();
        services.AddSingleton<DealsViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();

        return services.BuildServiceProvider();
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

    /// <summary>Disposes the tray icon and the service provider (which owns the HttpClient).</summary>
    public void Dispose()
    {
        _tray?.Dispose();
        _tray = null;
        _services?.Dispose();
        _services = null;
        GC.SuppressFinalize(this);
    }

    private void SetupTrayIcon()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Show rundown", null, (_, _) => ShowWindow());
        menu.Items.Add("Refresh now", null, (_, _) => _viewModel?.RefreshAll());
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
            Icon = AppIcon.LoadTrayIcon(),
            Text = "GameScout",
            Visible = true,
            ContextMenuStrip = menu,
        };
        _tray.DoubleClick += (_, _) => ShowWindow();
    }

    private void OnFreeScanCompleted(FreeGameReport report)
    {
        _log?.RecordFree(report);

        if (_tray is null)
            return;

        // Only surface a balloon when the window isn't already showing the result.
        bool windowVisible = _window is { IsVisible: true, WindowState: not WindowState.Minimized };
        if (windowVisible)
            return;

        int free = report.CurrentlyFree.Count();
        string title = free > 0
            ? $"{free} free game{(free == 1 ? string.Empty : "s")} available"
            : "No free games right now";
        string body = free > 0
            ? string.Join(", ", report.CurrentlyFree.Take(4).Select(g => g.Title))
            : "Nothing to grab at the moment.";

        _tray.BalloonTipTitle = title;
        _tray.BalloonTipText = body;
        _tray.ShowBalloonTip(6000);
    }
}
