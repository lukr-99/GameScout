# STRUCTURE

Repository layout and dependency intent.

```text
GameScout/
|- .github/
|  |- workflows/
|     |- ci.yml
|- src/
|  |- GameScout.Core/                (net10.0, platform-neutral, no WPF)
|  |  |- Abstractions/
|  |  |  |- IGiveawaySource.cs
|  |  |  |- IHttpTextClient.cs
|  |  |- Aggregation/
|  |  |  |- FreeGameAggregator.cs
|  |  |- Games/
|  |  |  |- FreeGame.cs
|  |  |  |- FreeGameReport.cs
|  |  |  |- GameStore.cs
|  |  |  |- GiveawayKind.cs
|  |  |- Mvvm/
|  |  |  |- ObservableObject.cs
|  |  |  |- RelayCommand.cs
|  |  |- Net/
|  |  |  |- HttpTextClient.cs
|  |  |- Sources/
|  |     |- Epic/
|  |     |  |- EpicFreeGamesSource.cs
|  |     |  |- EpicPromotionsDto.cs
|  |     |- GamerPower/
|  |        |- GamerPowerSource.cs
|  |        |- GamerPowerGiveawayDto.cs
|  |- GameScout.App/                  (net10.0-windows, WPF + WinForms tray)
|     |- Converters/
|     |  |- Converters.cs
|     |- Services/
|     |  |- StartupRegistration.cs
|     |  |- ThemeManager.cs
|     |- Themes/
|     |  |- Primitives.xaml
|     |  |- Theme.Light.xaml
|     |  |- Theme.Dark.xaml
|     |- ViewModels/
|     |  |- MainWindowViewModel.cs
|     |- App.xaml (+ .cs)
|     |- MainWindow.xaml (+ .cs)
|- tests/
|  |- Directory.Build.props
|  |- GameScout.Core.Tests/           (net10.0, xUnit)
|     |- Aggregation/
|     |- Sources/
|     |- Samples/                         (trimmed real API payloads)
|     |- TestSupport/
|- Directory.Build.props
|- GameScout.slnx
|- global.json
|- AGENTS.md / RULES.md / STRUCTURE.md
|- README.md / HANDOFF.md
|- INTENT.md / TODO.md / WORKLOG.md
```

## Dependency graph
```text
GameScout.App
  |- GameScout.Core

GameScout.Core.Tests
  |- GameScout.Core
```

`Core` never references the app or any WPF assembly. The app is the only Windows-specific project.
