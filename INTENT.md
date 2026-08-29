# INTENT

## Current intent
GameScout: a small WPF (.NET 10) Windows tray app that, on launch, reports **free-to-keep** games
(Epic/Steam/GOG/itch/… — "Free now" tab) and **popular games on sale** (with cover art and prices —
"On sale" tab), with a run-at-startup toggle and light/dark theming (including the native Win11
title bar). Built with DI + MVVM, platform-neutral tested Core, one class per file.

## Where things stand (2026-08-29, session 4)
- Core: model, giveaway + deal sources, aggregators (with **min-worth filter**), `AddGameScoutCore`
  DI module, and an **update checker** (GitHub Releases) — **done, tested (39/39)**.
- App: DI composition root, two-tab MVVM UI with image cards, themed Win11 chrome, tray (with update
  notification + open-log), icon, scan logging — **done, builds clean (0/0)**.
- **Installer + release automation scaffolded** (Inno Setup script, tag-triggered release workflow,
  RELEASING.md) — not yet run end-to-end on CI.
- **Verified live/headless** (free 13+1 after filtering, deals 32; update check runs). Visual
  rendering still needs a real display.

## Next steps
1. Cut a `v*` tag to exercise the release workflow and produce the first installer + GitHub Release.
2. Visual smoke test on a machine with a display (tabs, images, themed title bar, startup/tray).
3. Silent auto-update (download asset, run installer `/VERYSILENT`); settings UI for locale/min-worth.
