# INTENT

## Current intent
Deliver a small WPF (.NET 10) Windows tray app that, on launch, reports which normally-paid games
are currently free (and coming soon) on Epic and Steam, with a run-at-startup toggle and light/dark
themes. Architecture mirrors the `dotnetlib` conventions (platform-neutral tested `Core`, MVVM,
semantic theming).

## Where things stand (2026-08-29)
- Core domain + Epic source + GamerPower(Steam) source + parallel aggregator: **done, tested**.
- WPF app (window, tray, startup registration, theming, converters): **done, builds clean**.
- Build: 0 warnings / 0 errors. Tests: 13/13 passing.
- **Not yet manually smoke-tested** on real hardware (dev machine hardware failed). This is the
  first task on the next device — see `HANDOFF.md`.

## Next steps
1. Manual smoke test on the new machine (window populates, theme swap, startup toggle, tray/exit,
   open-in-browser). See HANDOFF.md checklist.
2. Address any runtime issues found (Epic UA/locale, tray balloon timing).
3. Optional polish: real app icon, configurable locale/country, periodic re-scan, more sources.
