# GameScout

A small **WPF (.NET 10) Windows tray app** that checks what games are **free to keep** right now —
and **coming soon** — on the **Epic Games Store** and **Steam**, then gives you a quick rundown at
startup. Glance at it, grab what you want, close it.

> New here or resuming on another machine? Read **[HANDOFF.md](HANDOFF.md)** first.

## Features
- **On-launch scan** of Epic + Steam for "normally paid, currently free" games.
- **Coming soon** section for games announced to go free later (Epic).
- **Tray app**: shows a rundown window and a notification-area balloon; closing the window hides to
  the tray, and the tray menu can refresh, toggle startup, or exit.
- **Run at Windows startup** toggle (per-user, no admin needed).
- **Light / dark** themes with a runtime toggle.
- Click any game to open its store/claim page in your browser.

## Requirements
- **Windows** (WPF + WinForms tray).
- **.NET 10 SDK** — pinned to `10.0.102` in `global.json`.

## Build & run
```bash
dotnet build GameScout.slnx -c Release
dotnet test  GameScout.slnx -c Release
dotnet run   --project src/GameScout.App
```
Start hidden in the tray (used by the startup entry): `dotnet run --project src/GameScout.App -- --tray`

## Architecture
Dependency direction is **App → Core**; `Core` has **no WPF dependency** and is fully unit-tested.

| Project | Target | Responsibility |
| --- | --- | --- |
| `src/GameScout.Core` | `net10.0` | Domain model, giveaway sources, aggregator, MVVM base |
| `src/GameScout.App` | `net10.0-windows` | WPF UI, tray icon, startup registration, theming |
| `tests/GameScout.Core.Tests` | `net10.0` | xUnit tests (deterministic, no network) |

See **[STRUCTURE.md](STRUCTURE.md)** for the full layout and **[RULES.md](RULES.md)** for the
engineering rules this repo follows.

## Data sources
Both are public and unauthenticated (no API key):
- **Epic Games Store** promotions feed — authoritative for Epic's weekly free games.
- **GamerPower** giveaways API (Steam filter) — catches normally-paid Steam games given away free.

## Status
Build clean, 13/13 tests passing. Not yet manually smoke-tested on real hardware — see HANDOFF.md.
