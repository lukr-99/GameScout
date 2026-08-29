# HANDOFF — GameScout

Snapshot for resuming this project on a different machine. Updated 2026-08-29 (session 5).

## Why this file exists
The machine this was built on had hardware trouble (graphics died mid-session). Everything is pushed
to a **private GitHub repo** (`lukr-99/GameScout`) so you can `git clone` on another device and keep
going with zero context loss.

## What GameScout is
A small **WPF + C# (.NET 10) Windows tray app** with two tabs:
- **Free now** — normally-paid games currently **free to keep** (Epic/Steam/GOG/itch/…), plus an
  *upcoming* section.
- **On sale** — popular **discounted** games with **cover art**, **sale + normal price**, and discount.

It shows a rundown window + tray balloon on launch, can **run at Windows startup**, and has
**light/dark themes** including the **native Win11 title bar/border** tinted to the theme. Built with
**DI (Microsoft.Extensions.DependencyInjection)** + **MVVM**, a platform-neutral tested `Core`, and a
one-class-per-file layout.

## Current status
- `dotnet build GameScout.slnx -c Release` → **0 warnings, 0 errors**
- `dotnet test GameScout.slnx -c Release` → **45/45 passing**
- **Data pipeline + DI + update checker verified live (headless):** free scan 13 free + 1 upcoming
  (after the min-worth filter), deals scan 32 on sale (see `%LOCALAPPDATA%\GameScout\scout.log`).
- **`v0.1.0` release verified end-to-end** on Windows CI with a published self-contained installer.
  Because the repo remains private, the app's anonymous update request currently receives 404;
  update discovery needs a public release endpoint or explicit user authentication.
- **Settings UI:** locale/country/minimum-worth values persist to
  `%LOCALAPPDATA%\GameScout\settings.json` and apply on the next launch.
- **Visual rendering not yet eyeballed** on a real display — a top task below.

## How to resume (new machine)
1. Install the **.NET 10 SDK** (`10.0.102`, per `global.json`). Windows only (WPF + WinForms tray;
   Win11 recommended for the themed title bar).
2. `git clone https://github.com/lukr-99/GameScout.git && cd GameScout`
3. `dotnet build GameScout.slnx -c Release` and `dotnet test GameScout.slnx -c Release`
4. Run: `dotnet run --project src/GameScout.App` (add `-- --tray` to start hidden in the tray).

## First thing to do on the new machine — visual smoke test
Everything behind the UI is proven; confirm the pixels:
- Window renders; **both tabs** ("Free now", "On sale") populate; **cover images** load.
- **Theme** button flips light/dark AND re-tints the **native title bar/border** (DWM).
- **Run at startup** toggles `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\GameScout`.
- Close hides to tray; tray **Exit** quits; a card click opens the store page.

## Architecture (App → Core)
```
src/GameScout.Core   (net10.0, NO WPF)   model, sources, aggregators, DI module, MVVM base
src/GameScout.App    (net10.0-windows)   WPF tabs/cards/theming/tray, DI composition root
tests/GameScout.Core.Tests (net10.0)     xUnit, deterministic (no network)
```
- **DI:** `Core/DependencyInjection/ServiceCollectionExtensions.AddGameScoutCore` registers sources,
  aggregators, `TimeProvider`, and `GameScoutOptions` (locale/country). `App.xaml.cs` builds the
  `ServiceProvider`, registers `HttpClient` + `IHttpTextClient`, and ctor-injects the view-models and
  `MainWindow`.
- **Sources (`Core/Sources`):**
  - `Epic/EpicFreeGamesSource` — free when `discountPercentage == 0`; images from `keyImages`.
  - `GamerPower/GamerPowerSource` — all-platform paid-now-free giveaways; skips Epic (dedupe); images.
  - `CheapShark/CheapSharkSource` — discounted games with prices, discount %, store map, cover images
    (prefers the Steam header image when a steamAppID is present).
- **Aggregators:** `GiveawayAggregator` (free, parallel, dedupe by title/store/kind, expiry filter)
  and `DealAggregator` (deals, dedupe by title keeping deepest discount, capped at 60).
- **App view-models:** `ScannerViewModel` base (busy/error/status/refresh) → `FreeGamesViewModel`,
  `DealsViewModel`; `MainWindowViewModel` shell; `SettingsViewModel` validates persisted options.
- **Services:** `ThemeManager` (runtime theme swap + event), `WindowChromeThemer` (DWM title-bar
  tint), `StartupRegistration` (HKCU Run key), `ScanLog`, `UrlOpener`, `AppIcon`.

## Data sources (public, no API key)
- Epic promotions feed; GamerPower giveaways (`?type=game`); CheapShark deals
  (`?sortBy=Deal Rating&onSale=1`). A **descriptive User-Agent is required by CheapShark** and is set
  by the app's `HttpClient`.

## Releases & auto-update
- `Core/Updating` checks the latest GitHub Release and compares to the running version. On launch the
  app notifies via the tray and offers **Download update**.
- Cutting a release is automated: bump `<Version>` in `Directory.Build.props`, then
  `git tag vX.Y.Z && git push origin vX.Y.Z`. The **Release** workflow publishes, builds the Inno
  Setup installer, and attaches it to a GitHub Release. `v0.1.0` completed successfully. Full
  details and the private-repository update limitation are in `docs/RELEASING.md`.

## Known gaps / next slices (see TODO.md)
- Visual smoke test still pending (above).
- Update discovery and silent auto-update need an anonymously accessible release endpoint or user auth.
- No app-layer view-model tests (Core is covered).

## Verification baseline (before committing)
```bash
dotnet build GameScout.slnx -c Release
dotnet test  GameScout.slnx -c Release
```
