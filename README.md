# GameScout

A small **WPF (.NET 10) Windows tray app** that scouts what's worth grabbing on PC storefronts — the
games that are **free to keep** right now (and coming soon), plus **popular games on sale** — and
gives you a quick, glanceable rundown at startup.

> New here or resuming on another machine? Read **[HANDOFF.md](HANDOFF.md)** first.

![GameScout icon](src/GameScout.App/Assets/gamescout.png)

## Features
- **Two tabs:**
  - **Free now** — normally-paid games currently free to keep, plus an *upcoming* section.
  - **On sale** — popular discounted games with **cover art**, **sale + normal price**, and the discount.
- **Many storefronts:** Epic (direct) and — via aggregators — Steam, GOG, Humble, Fanatical,
  Prime Gaming, itch.io and more; deals span Steam/GOG/Epic/Humble/Fanatical/GMG/… (CheapShark).
- **Tray app** with a startup balloon; closing the window hides to the tray.
- **Run at Windows startup** toggle (per-user, no admin).
- **Light / dark themes** with a runtime toggle — including the **native Win11 title bar & border**,
  which are tinted to match the theme.
- **Quality filter:** trivial freebies below a configurable price (default $2.99) are hidden.
- **Auto-update check:** on launch it checks GitHub Releases and offers the new installer from the tray.
- Click any game to open its store/claim page in your browser.

## Requirements
- **Windows 11** (uses DWM window-chrome theming; degrades gracefully on older Windows).
- **.NET 10 SDK** — pinned to `10.0.102` in `global.json`.

## Build & run
```bash
dotnet build GameScout.slnx -c Release
dotnet test  GameScout.slnx -c Release
dotnet run   --project src/GameScout.App
```
Start hidden in the tray (used by the startup entry): `dotnet run --project src/GameScout.App -- --tray`

## Architecture
Dependency direction is **App → Core**; `Core` has **no WPF dependency**, is fully unit-tested, and
is wired with **Microsoft.Extensions.DependencyInjection** via `AddGameScoutCore`. MVVM throughout,
one class per file. See **[STRUCTURE.md](STRUCTURE.md)** and **[RULES.md](RULES.md)**.

| Project | Target | Responsibility |
| --- | --- | --- |
| `src/GameScout.Core` | `net10.0` | Model, giveaway/deal sources, aggregators, DI module, MVVM base |
| `src/GameScout.App` | `net10.0-windows` | WPF UI (tabs, cards, theming, tray), composition root |
| `tests/GameScout.Core.Tests` | `net10.0` | xUnit tests (deterministic, no network) |

## Data sources (public, no API key)
- **Epic Games Store** promotions feed — authoritative for Epic's weekly free games.
- **GamerPower** giveaways API — paid games given away free across many stores (Epic excluded to
  avoid duplicates).
- **CheapShark** deals API — popular discounted games with images and prices across many stores.

A descriptive `User-Agent` is sent (required by CheapShark).

## Releases
Installers are produced automatically by tagging (`vX.Y.Z`) — see **[docs/RELEASING.md](docs/RELEASING.md)**.
The app checks GitHub Releases for updates on launch.

## Status
Build clean, 39/39 tests passing; data pipeline + DI + update checker verified against live APIs.
Visual rendering is best confirmed on a real display — see HANDOFF.md.
