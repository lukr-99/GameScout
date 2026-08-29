# STRUCTURE

Repository layout and dependency intent.

```text
GameScout/
|- .github/workflows/ci.yml
|- src/
|  |- GameScout.Core/                     (net10.0, platform-neutral, no WPF)
|  |  |- Abstractions/
|  |  |  |- IGiveawaySource.cs            (free-game sources)
|  |  |  |- IDealSource.cs                (on-sale sources)
|  |  |  |- IHttpTextClient.cs
|  |  |- Aggregation/
|  |  |  |- GiveawayAggregator.cs         (free games, parallel fan-out)
|  |  |  |- DealAggregator.cs             (deals, dedupe by title, deepest first)
|  |  |- DependencyInjection/
|  |  |  |- GameScoutOptions.cs
|  |  |  |- ServiceCollectionExtensions.cs (AddGameScoutCore)
|  |  |- Games/                           (FreeGame, GameDeal, reports, GameStore(+names))
|  |  |- Mvvm/                            (ObservableObject, RelayCommand)
|  |  |- Net/HttpTextClient.cs
|  |  |- Sources/
|  |     |- Epic/                         (free games + images)
|  |     |- GamerPower/                   (all-platform giveaways + images, skips Epic)
|  |     |- CheapShark/                   (deals: prices, discount, images, store map)
|  |- GameScout.App/                      (net10.0-windows, WPF + WinForms tray)
|     |- Assets/gamescout.ico             (app + window + tray icon)
|     |- Converters/Converters.cs
|     |- Services/                        (ThemeManager, WindowChromeThemer, StartupRegistration,
|     |                                    ScanLog, UrlOpener, AppIcon)
|     |- ViewModels/                      (ScannerViewModel base, FreeGamesViewModel,
|     |                                    DealsViewModel, MainWindowViewModel, ScanOutcome)
|     |- Views/                           (FreeGamesView, DealsView user controls)
|     |- Themes/                          (Theme.Light, Theme.Dark, Primitives)
|     |- App.xaml (+ .cs)                 (DI composition root + tray)
|     |- MainWindow.xaml (+ .cs)          (shell: top bar + tabs + footer)
|- tests/
|  |- GameScout.Core.Tests/              (net10.0, xUnit; Samples/ fixtures, TestSupport/ stubs)
|- Directory.Build.props / global.json / GameScout.slnx
|- README / HANDOFF / RULES / AGENTS / STRUCTURE / INTENT / TODO / WORKLOG
```

## Dependency graph
```text
GameScout.App  ->  GameScout.Core  (via Microsoft.Extensions.DependencyInjection)
GameScout.Core.Tests  ->  GameScout.Core
```

`Core` references only `Microsoft.Extensions.DependencyInjection.Abstractions` (for the
`AddGameScoutCore` module) and stays free of any WPF/WinForms dependency. The app builds the
`ServiceProvider` and registers the `HttpClient` + `IHttpTextClient` the sources depend on.
