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
- [ ] **Installer + release automation** — scaffolding landed (`installer/GameScout.iss`,
      `.github/workflows/release.yml`, `docs/RELEASING.md`). **Next:** cut a real `v*` tag to run the
      workflow end-to-end and confirm the installer + Release asset (needs a Windows CI run).
- [ ] **Silent auto-update:** download the installer asset and run it with `/VERYSILENT` instead of
      opening the browser (URL already exposed via `ReleaseInfo.DownloadUrl`).
- [ ] Settings UI for locale/country + min-worth (Core already supports them via `GameScoutOptions`).
- [ ] Cache/placeholder for cover images while they download; handle offline gracefully in the UI.

## Later / optional
- [ ] Add a direct Steam specials source (reduce reliance on CheapShark/GamerPower).
- [ ] Optional periodic re-scan while docked in the tray.
- [ ] App-layer view-model tests (Core is covered; App is currently thin).
