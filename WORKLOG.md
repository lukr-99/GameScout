# WORKLOG

## 2026-08-29 — Initial build
- Established repo scaffolding mirroring `dotnetlib` conventions: `global.json` (SDK 10.0.102),
  root + tests `Directory.Build.props` (nullable, implicit usings, warnings-as-errors,
  latest-recommended analysis), `.slnx` solution, `.gitignore`.
- **Core (net10.0, platform-neutral):**
  - MVVM base (`ObservableObject`, `RelayCommand`).
  - Domain model: `FreeGame`, `GameStore`, `GiveawayKind`, `FreeGameReport`.
  - Abstractions: `IGiveawaySource`, `IHttpTextClient`; `HttpTextClient` transport.
  - `EpicFreeGamesSource` — parses the live Epic promotions feed; free = promo
    `discountPercentage == 0`; handles current + upcoming; filters F2P; builds store URLs from slugs.
  - `GamerPowerSource` — Steam giveaways of normally-paid games; filters out `N/A`/$0 worth; cleans
    titles; parses end dates.
  - `FreeGameAggregator` — parallel fan-out, per-source error capture, de-dupe, relevance filter,
    display ordering; injectable `TimeProvider`.
- **App (net10.0-windows, WPF + WinForms tray):**
  - Hand-wired composition root in `App.xaml.cs`; owns `HttpClient` + `NotifyIcon`; first scan on
    launch; tray balloon when window hidden; `--tray` starts hidden.
  - `MainWindowViewModel` with refresh / open-in-browser / run-at-startup / theme-toggle.
  - `ThemeManager` (runtime light/dark swap), `StartupRegistration` (HKCU Run key).
  - Semantic light/dark theme dictionaries + primitives; clickable game-card template; converters.
- **Tests:** 13 xUnit tests (Epic parse, GamerPower parse, aggregator merge/dedupe/order/expire),
  deterministic via `FakeTimeProvider` and trimmed real-payload fixtures. All green.
- Verified against **live** Epic and GamerPower endpoints while building the parsers.
- Build: 0 warnings / 0 errors. Tests: 13/13.
- Dev machine hardware began failing — created `HANDOFF.md` and pushed to a private GitHub repo so
  work can continue on another device. **Manual interactive smoke test still pending.**
