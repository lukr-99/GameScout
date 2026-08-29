# TODO

Actionable next slices. Check off as delivered.

## Do first (on the new machine)
- [x] **Data-pipeline smoke test** — verified end-to-end via a live headless run: both sources hit
      real APIs, aggregator merged 3 free + 1 upcoming, no errors. See the scan log at
      `%LOCALAPPDATA%\GameScout\scout.log`.
- [ ] **Visual smoke test (still pending — needs a real display):** confirm the window renders and
      the cards look right. Everything behind the UI is proven; only pixels are unverified.
- [ ] Verify **theme toggle** swaps light/dark correctly at runtime.
- [ ] Verify **Run at startup** writes/removes `HKCU\...\Run\GameScout`, and that the
      `--tray` launch comes up hidden with a balloon.
- [ ] Verify **close → hides to tray**, tray **Exit** quits, and a card click opens the browser.

## Near term
- [ ] Add a real application icon (`.ico`) and use it for the window + tray instead of the system icon.
- [x] Make Epic locale/country configurable (`EpicFreeGamesSource(http, locale, country)` +
      `BuildEndpoint`). Still wired with en-US/US defaults in `App`; expose a settings UI later.
- [ ] Surface the scan log path in the UI (e.g. a tray menu item "Open log") for easy troubleshooting.
- [ ] Handle the "no internet" case with a friendlier inline message (aggregator already degrades,
      but confirm the UX).

## Later / optional
- [ ] Add a direct Steam source (e.g. store search for 100%-off) to reduce reliance on GamerPower.
- [ ] Add GOG free-games coverage (GamerPower already exposes it).
- [ ] Optional periodic re-scan (e.g. every few hours) while the app sits in the tray.
- [ ] Consider a small App-layer test project for view-model behavior (Core is already covered).
- [ ] Package as a single-file self-contained publish for easy install on other machines.
