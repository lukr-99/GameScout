# STRUCTURE

Repository layout and dependency intent.

```text
FreeGameScout/
|- .github/
|  |- workflows/
|     |- ci.yml
|- src/
|  |- FreeGameScout.Core/                (net10.0, platform-neutral, no WPF)
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
|  |- FreeGameScout.App/                  (net10.0-windows, WPF + WinForms tray)
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
|  |- FreeGameScout.Core.Tests/           (net10.0, xUnit)
|     |- Aggregation/
|     |- Sources/
|     |- Samples/                         (trimmed real API payloads)
|     |- TestSupport/
|- Directory.Build.props
|- FreeGameScout.slnx
|- global.json
|- AGENTS.md / RULES.md / STRUCTURE.md
|- README.md / HANDOFF.md
|- INTENT.md / TODO.md / WORKLOG.md
```

## Dependency graph
```text
FreeGameScout.App
  |- FreeGameScout.Core

FreeGameScout.Core.Tests
  |- FreeGameScout.Core
```

`Core` never references the app or any WPF assembly. The app is the only Windows-specific project.
