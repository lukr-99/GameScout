# INTENT

## Current intent
GameScout: a small WPF (.NET 10) Windows tray app that, on launch, reports **free-to-keep** games
(Epic/Steam/GOG/itch/… — "Free now" tab) and **popular games on sale** (with cover art and prices —
"On sale" tab), with a run-at-startup toggle and light/dark theming (including the native Win11
title bar). Built with DI + MVVM, platform-neutral tested Core, one class per file.

## Where things stand (2026-08-29, session 3)
- Core: model, giveaway + deal sources (Epic, GamerPower, CheapShark), aggregators, `AddGameScoutCore`
  DI module — **done, tested (28/28)**.
- App: DI composition root, two-tab MVVM UI with image cards, themed Win11 chrome, tray, icon,
  scan logging — **done, builds clean (0/0)**.
- **Data pipeline + DI verified live/headless** (free 17+1, deals 32). The **visual rendering** is
  the only piece not yet eyeballed (needs a display).

## Next steps
1. Visual smoke test on a machine with a display (tabs, images, themed title bar, startup/tray).
2. Trim free-game noise (min-worth filter for itch.io freebies) and add a settings UI for locale.
3. Optional: direct Steam specials source, periodic re-scan, single-file publish.
