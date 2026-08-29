# TODO

Actionable next slices. Check off as delivered.

## Do first (needs a real display)
- [x] Data-pipeline smoke test — verified live & headless (free + deals scans, DI graph resolves).
      See `%LOCALAPPDATA%\GameScout\scout.log`.
- [ ] **Visual smoke test:** confirm the window renders, both **tabs** work, **cover images** load,
      the **themed Win11 title bar/border** tints correctly and re-tints on theme toggle.
- [ ] Verify **Run at startup** writes/removes `HKCU\...\Run\GameScout`, and `--tray` starts hidden.
- [ ] Verify **close → hides to tray**, tray **Exit** quits, and a card click opens the browser.

## Near term
- [ ] Quality filter for free games: the broadened GamerPower feed surfaces low-value itch.io
      freebies (e.g. tiny jam games). Consider a min-worth threshold or per-store toggles.
- [ ] Settings UI for locale/country (Core already supports it via `GameScoutOptions`).
- [ ] "Open log" tray item for easy troubleshooting.
- [ ] Cache/placeholder for cover images while they download; handle offline gracefully in the UI.

## Later / optional
- [ ] Add a direct Steam specials source (reduce reliance on CheapShark/GamerPower).
- [ ] Optional periodic re-scan while docked in the tray.
- [ ] App-layer view-model tests (Core is covered; App is currently thin).
- [ ] Single-file self-contained publish for easy install on other machines.
