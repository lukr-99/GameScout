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
- [x] Min-worth filter for free games (`GameScoutOptions.MinimumWorth`, default $2.99) — trims
      trivial itch.io freebies; live run dropped 17 → 13. Unknown-price offers are always kept.
- [x] "Open log folder" tray item.
- [x] In-app **update check** (GitHub Releases) with tray notification + "Download update" item.
- [x] **Installer + release automation** — `v0.1.0` completed successfully on Windows CI and
      published `GameScoutSetup-0.1.0.exe`. The installer now also creates a root Start Menu shortcut
      and per-user App Paths registration for reliable Windows launching.
- [ ] **Choose a public release endpoint or authentication model.** The private GitHub repo returns
      404 to the app's anonymous update request; do not embed a repository token.
- [ ] **Silent auto-update:** after release assets are accessible, securely download the installer
      and run it with `/VERYSILENT` instead of opening the browser.
- [x] Settings UI for locale/country + min-worth, persisted under `%LOCALAPPDATA%\GameScout` and
      loaded when the app next starts.
- [ ] Cache/placeholder for cover images while they download; handle offline gracefully in the UI.

## Later / optional
- [ ] Add a direct Steam specials source (reduce reliance on CheapShark/GamerPower).
- [ ] Optional periodic re-scan while docked in the tray.
- [ ] App-layer view-model tests (Core is covered; App is currently thin).
