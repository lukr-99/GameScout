# Releasing GameScout

GameScout ships as a per-user Windows installer, and the app checks GitHub Releases for updates on
launch. Cutting a release is fully automated by tagging.

## How a release works
1. Bump `<Version>` in `Directory.Build.props` (e.g. `0.3.0`) and commit.
2. Tag and push:
   ```bash
   git tag v0.3.0
   git push origin v0.3.0
   ```
3. The **Release** workflow (`.github/workflows/release.yml`) runs on the tag:
   - runs the tests,
   - `dotnet publish` the app self-contained for `win-x64`,
   - builds `GameScoutSetup-<version>.exe` with Inno Setup,
   - creates a GitHub Release and attaches the installer.
4. On next launch, existing installs see the newer tag via the in-app **update checker**
   (`GameScout.Core.Updating`), show a tray notification, and the tray menu offers **Download update**
   (which points at the installer asset). Re-running the installer upgrades in place.

## Building the installer locally (Windows)
```bash
dotnet publish src/GameScout.App/GameScout.App.csproj -c Release -r win-x64 --self-contained true -o publish
iscc /DAppVersion=0.3.0 installer/GameScout.iss   # needs Inno Setup 6 (ISCC.exe on PATH)
```
The installer lands in `installer/dist/`.

## Versioning
- App/assembly version comes from `Directory.Build.props` `<Version>`; CI overrides it from the tag.
- The update checker compares `major.minor.build` of the running assembly against the latest release
  tag (a leading `v` and any pre-release suffix are ignored). Keep tags as `vX.Y.Z`.

## Next step toward silent auto-update
Today the app *notifies* and opens the installer download. A future iteration can download the asset
to a temp folder and launch it with `/VERYSILENT` (Inno's silent flag) to update without the browser
round-trip — the `ReleaseInfo.DownloadUrl` already exposes the asset URL for this.
