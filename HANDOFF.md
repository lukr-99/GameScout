# HANDOFF — GameScout

Snapshot for resuming this project on a different machine. Written 2026-08-29.

## Why this file exists
The machine this was built on started losing hardware (graphics died). Everything below is
pushed to a **private GitHub repo** so you can `git clone` on another device and keep going with
zero context loss.

## What GameScout is
A small **WPF + C# (.NET 10) Windows tray app** that, on launch, checks what games are **free to
keep right now** (and **coming soon**) on the **Epic Games Store** and **Steam**, shows a quick
rundown window + a tray balloon, and can register itself to **run at Windows startup**. You glance
at it and close it (it hides to the tray; full quit is in the tray menu).

It was built to follow the architecture conventions in the sibling `dotnetlib` repo
(`F:\Code\dotnetlib`): a platform-neutral `Core` with no WPF dependency, MVVM with
`ObservableObject`/`RelayCommand`, one class per file, nullable enabled, warnings-as-errors,
semantic theme keys with light + dark, and a test project per production layer.

## Current status — WORKING ✅
- `dotnet build GameScout.slnx -c Release` → **0 warnings, 0 errors**
- `dotnet test GameScout.slnx -c Release` → **13/13 passing**
- The app compiles and runs; it has **not yet been launched interactively** on real hardware
  (the machine failed before a manual smoke test). See "First thing to do on the new machine".

## How to resume (new machine)
1. Install the **.NET 10 SDK** (pinned to `10.0.102` in `global.json`, `rollForward: latestFeature`).
   This is a **Windows-only** app (WPF + WinForms tray) — build/run on Windows.
2. `git clone <your private repo URL>` then `cd GameScout`.
3. Build + test:
   ```bash
   dotnet build GameScout.slnx -c Release
   dotnet test GameScout.slnx -c Release
   ```
4. Run the app:
   ```bash
   dotnet run --project src/GameScout.App
   ```
   A window should appear, scan Epic + Steam, and list free games. A tray icon appears in the
   notification area. Pass `--tray` to start hidden (this is what the startup entry uses).

## First thing to do on the new machine
**Manual smoke test** (never run interactively yet):
- Launch and confirm the window populates with real free games (needs internet).
- Toggle **Theme** (light/dark) and confirm colors swap.
- Toggle **Run at startup**, then check the registry value exists:
  `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` → value name `GameScout`.
  Untoggle and confirm it's removed. (Uses HKCU only — no admin needed.)
- Close the window → app should hide to tray, not exit. Tray → **Exit** quits fully.
- Double-click a game card → opens its store/claim page in the browser.

## Architecture (dependency direction: App → Core)
```
src/GameScout.Core   (net10.0, NO WPF)   domain model, sources, aggregator, MVVM base
src/GameScout.App    (net10.0-windows)   WPF UI, tray, startup registration, theming
tests/GameScout.Core.Tests (net10.0)     xUnit, deterministic (no network)
```

### Core pieces
- `Games/FreeGame.cs` — normalized offer record (title, store, kind, url, price, start/end UTC).
- `Games/GameStore.cs`, `Games/GiveawayKind.cs` — enums.
- `Games/FreeGameReport.cs` — scan result: games + per-source errors + timestamp.
- `Abstractions/IGiveawaySource.cs` — one storefront/feed. Never throws for "nothing found".
- `Abstractions/IHttpTextClient.cs` + `Net/HttpTextClient.cs` — HTTP transport, abstracted so
  sources are unit-testable against canned JSON (no network in tests).
- `Sources/Epic/EpicFreeGamesSource.cs` — parses Epic's public promotions feed. **Key rule:**
  an offer is free when `discountSetting.discountPercentage == 0` (Epic's schema quirk: 0% of the
  price remains = free). Current freebies come from `promotionalOffers`, upcoming from
  `upcomingPromotionalOffers`. Free-to-play (originalPrice 0) is filtered out.
- `Sources/GamerPower/GamerPowerSource.cs` — uses the public GamerPower API
  (`?platform=steam&type=game`) to catch normally-paid Steam games that are temporarily free.
  Filters out entries whose `worth` is `N/A`/$0 (i.e. real F2P).
- `Aggregation/FreeGameAggregator.cs` — fans out to all sources **in parallel**, records a
  per-source error instead of failing the whole scan, de-dupes by (title, store, kind), drops
  expired/past offers, and orders currently-free before upcoming.

### App pieces
- `App.xaml.cs` — composition root (hand-wired, no DI container), owns `HttpClient` + tray icon,
  runs the first scan on launch, shows a balloon when the window is hidden.
- `MainWindowViewModel.cs` — refresh / open-in-browser / run-at-startup / theme-toggle commands.
- `Services/ThemeManager.cs` — swaps the theme dictionary at runtime (index 0 of merged dicts).
- `Services/StartupRegistration.cs` — HKCU Run-key registration; startup launch adds `--tray`.
- `Themes/Theme.Light.xaml` / `Theme.Dark.xaml` / `Primitives.xaml` — semantic keys only.
- `Converters/Converters.cs` — store label, offer timing text, visibility helpers.

## Data sources (both public, unauthenticated, no API key)
- **Epic:** `https://store-site-backend-static-ipv4.ak.epicgames.com/freeGamesPromotions?locale=en-US&country=US&allowCountries=US`
- **GamerPower (Steam):** `https://www.gamerpower.com/api/giveaways?platform=steam&type=game`

Both were live-verified during the build; the parsers were written against **real** responses and
the test fixtures in `tests/GameScout.Core.Tests/Samples/` are trimmed real payloads.

## Known gaps / next slices (see TODO.md for the full list)
- No manual/interactive smoke test yet (do this first — see above).
- Steam "went free" coverage relies on GamerPower; there's no direct Steam store scrape yet.
- No app icon (tray uses the generic system icon) — add a real `.ico` when convenient.
- Country/locale is hard-coded to en-US/US in the Epic source; consider making it configurable.
- No UI test project yet (Core is covered; App is thin view wiring). Optional.
- Consider a periodic re-scan (currently one scan per launch; refresh is manual).

## Verification baseline (run before committing)
```bash
dotnet build GameScout.slnx -c Release
dotnet test  GameScout.slnx -c Release
```
(A `dotnet format` / analyzer pass also runs in CI — see `.github/workflows/ci.yml`.)
