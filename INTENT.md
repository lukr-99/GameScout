# INTENT

## Current intent
Deliver a small WPF (.NET 10) Windows tray app that, on launch, reports which normally-paid games
are currently free (and coming soon) on Epic and Steam, with a run-at-startup toggle and light/dark
themes. Architecture mirrors the `dotnetlib` conventions (platform-neutral tested `Core`, MVVM,
semantic theming).

## Where things stand (2026-08-29, session 2)
- Core domain + Epic source + GamerPower(Steam) source + parallel aggregator: **done, tested**.
- WPF app (window, tray, startup registration, theming, converters, scan logging): **done, builds clean**.
- Build: 0 warnings / 0 errors. Tests: **15/15** passing.
- **Data pipeline smoke-tested live** on real hardware via a headless `--tray` run (3 free + 1
  upcoming, no errors — see `scout.log`). The **WPF window rendering** is the only piece not yet
  eyeballed (needs a real display).

## Next steps
1. Visual smoke test on a machine with a display: confirm the window/cards render correctly, theme
   toggle works, startup toggle writes/removes the registry value, close→tray + tray Exit behave.
2. Optional polish: real app icon, "Open log" tray item, settings UI for locale/country, periodic
   re-scan, more sources (direct Steam / GOG).
