# GameScout

GameScout is a small Windows tray app. It checks PC game stores and shows two lists: games you can
keep for free right now, and popular games on sale. It shows a short summary when it starts, so you
can see what is worth grabbing without opening a store.

## Status

- Version 0.3.0, tagged 2026-08-30. See the [releases](https://github.com/lukr-99/GameScout/releases).
- The solution builds with 0 warnings and 0 errors. The 57 tests in `tests/GameScout.Core.Tests`
  pass (checked 2026-11-26).
- Not yet checked on a real display: the visual layout, tray behavior, and the themed title bar.
  See [HANDOFF.md](HANDOFF.md) for the open checks.

## Features

- **Free now tab:** games that are free to keep right now, plus a section for upcoming free games.
- **On sale tab:** popular discounted games with cover art, the sale price, the normal price, and
  the discount.
- **Tray app:** a balloon summary on start. Closing the window hides it to the tray.
- **Run at Windows startup:** a toggle for the current user. It needs no admin rights.
- **Light and dark themes:** you can switch at runtime. The Windows 11 title bar follows the theme.
- **Quality filter:** free games below a minimum price are hidden. You can change the minimum in
  Settings.
- **Saved settings:** the Epic locale, the country, and the minimum price are stored in
  `%LOCALAPPDATA%\GameScout\settings.json`.
- **Update check:** on start, the app checks GitHub Releases and offers a newer installer from the
  tray.
- **About page:** version, author, feedback links, and credits.
- Click a game to open its store or claim page in your browser.

## Requirements

- Windows 11. The app uses Windows 11 title bar theming. It still runs on older Windows, but the
  title bar will not match the theme.
- The .NET 10 SDK, to build from source. `global.json` pins SDK 10.0.102 and allows newer feature
  bands.
- An internet connection. The app reads public data and needs no API key.

## Quick start

From the repository root:

```powershell
dotnet build GameScout.slnx -c Release
dotnet run --project src/GameScout.App
```

To start hidden in the tray, the way the startup entry does, add `--tray`:

```powershell
dotnet run --project src/GameScout.App -- --tray
```

## Build and test

Run these from the repository root. Warnings are errors in this repository.

```powershell
dotnet build GameScout.slnx -c Release
dotnet test GameScout.slnx -c Release
```

## Data sources

The app reads three public sources. None needs an API key.

| Source | Used for |
| --- | --- |
| Epic Games Store promotions feed | Epic's weekly free games. This is the main source for Epic. |
| GamerPower giveaways API | Paid games given away free on other stores. Epic is left out to avoid duplicates. |
| CheapShark deals API | Discounted games with images and prices from many stores. |

The app sends a descriptive `User-Agent` header, which CheapShark asks for.

## Architecture

The app has three projects. `src/GameScout.Core` holds the models, sources, and aggregation, and has
no WPF dependency. `src/GameScout.App` holds the WPF UI, the tray, and the composition root.
`tests/GameScout.Core.Tests` holds the unit tests, which do not use the network.

The dependency direction is App to Core. See [STRUCTURE.md](STRUCTURE.md) for the full layout and
[RULES.md](RULES.md) for the coding rules.

## Data safety

- The app only reads the public sources above. Clicking a game opens its page in your browser.
- Settings are saved under `%LOCALAPPDATA%\GameScout\`. The log is `scout.log` in the same folder.
- Startup registration writes to the current user's registry hive (`HKCU`) only.

## Delivery

Pushing a tag such as `v0.3.0` starts the release workflow in `.github/workflows/release.yml`. It runs
the tests, publishes a self-contained Windows build, builds the Inno Setup installer, and creates a
GitHub Release with the installer attached. The steps are in [docs/RELEASING.md](docs/RELEASING.md).

## License

No LICENSE file is in this repository yet, so no reuse terms are granted.
