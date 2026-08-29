# INTENT

## Current intent
GameScout: a small WPF (.NET 10) Windows tray app that, on launch, reports **free-to-keep** games
(Epic/Steam/GOG/itch/… — "Free now" tab) and **popular games on sale** (with cover art and prices —
"On sale" tab), with a run-at-startup toggle and light/dark theming (including the native Win11
title bar). Built with DI + MVVM, platform-neutral tested Core, one class per file.

## Where things stand (2026-08-29, session 5)
- Core: model, giveaway + deal sources, aggregators (with **min-worth filter**), `AddGameScoutCore`
  DI module, validated JSON settings, and an **update checker** (GitHub Releases) — **done, tested
  (45/45)**.
- App: DI composition root, two-tab MVVM UI with image cards, themed Win11 chrome, tray (with update
  notification + open-log), icon, scan logging, and persisted locale/country/min-worth settings —
  **done, builds clean (0/0)**.
- **Installer + release automation verified:** `v0.1.0` published successfully. The dev version is
  now `0.2.0`; Start Menu/App Paths registration is improved for the next installer.
- **Private-repo limitation confirmed:** anonymous update discovery returns 404. Silent update is
  deferred until binaries have a public endpoint or explicit user authentication.
- **Verified live/headless** (free 13+1 after filtering, deals 32; update check runs). Visual
  rendering still needs a real display.

## Next steps
1. Visual smoke test on a machine with a display (tabs, images, settings, themed title bar, startup/tray).
2. Choose a public binary endpoint or explicit authentication design for update discovery.
3. Add cover-image caching/placeholders and improve offline UI behavior.
