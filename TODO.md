# TODO

Actionable next slices. Check off as delivered.

## Do first (on the new machine)
- [ ] **Manual smoke test** — launch, confirm the list populates from live Epic + Steam data.
- [ ] Verify **theme toggle** swaps light/dark correctly at runtime.
- [ ] Verify **Run at startup** writes/removes `HKCU\...\Run\FreeGameScout`, and that the
      `--tray` launch comes up hidden with a balloon.
- [ ] Verify **close → hides to tray**, tray **Exit** quits, and a card click opens the browser.

## Near term
- [ ] Add a real application icon (`.ico`) and use it for the window + tray instead of the system icon.
- [ ] Make Epic locale/country configurable (currently hard-coded en-US/US).
- [ ] Handle the "no internet" case with a friendlier inline message (aggregator already degrades,
      but confirm the UX).

## Later / optional
- [ ] Add a direct Steam source (e.g. store search for 100%-off) to reduce reliance on GamerPower.
- [ ] Add GOG free-games coverage (GamerPower already exposes it).
- [ ] Optional periodic re-scan (e.g. every few hours) while the app sits in the tray.
- [ ] Consider a small App-layer test project for view-model behavior (Core is already covered).
- [ ] Package as a single-file self-contained publish for easy install on other machines.
