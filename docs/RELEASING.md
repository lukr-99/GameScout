# Releasing GameScout

GameScout ships as a per-user Windows installer, and the app checks GitHub Releases for updates on
launch. Cutting a release is fully automated by tagging.

> **Update discovery:** the repository is public, so the anonymous release API and asset downloads
> the app relies on both work — installed builds discover new tags on launch with no embedded
> credential. (While the repo was private these returned 404; keep the release repo public, or add
> explicit user authentication, for discovery to keep working.)

## How a release works
1. Bump `<VersionPrefix>` in `Directory.Build.props` (e.g. `0.3.0`) and commit.
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
4. Existing installs see the newer tag via the in-app **update checker** (`GameScout.Core.Updating`),
   show a tray notification, and the tray menu offers **Download update** (which points at the
   installer asset). Re-running the installer upgrades in place.

## Building the installer locally (Windows)
```bash
dotnet publish src/GameScout.App/GameScout.App.csproj -c Release -r win-x64 --self-contained true -o publish
iscc /DAppVersion=0.3.0 installer/GameScout.iss   # needs Inno Setup 6 (ISCC.exe on PATH)
```
The installer lands in `installer/dist/`.

## Versioning
- App/assembly version comes from `Directory.Build.props` `<VersionPrefix>`; CI overrides it from the
  tag via `-p:Version=<tag>`.
- Debug builds append a `-dev` suffix (`<VersionSuffix>`), so a locally-run test instance shows
  e.g. `GameScout 0.3.0-dev` in its title bar and tray tooltip while a release shows `GameScout 0.3.0`.
  The suffix only affects the informational version; `AssemblyVersion` stays numeric.
- The update checker compares `major.minor.build` of the running assembly against the latest release
  tag (a leading `v` and any pre-release suffix are ignored). Keep tags as `vX.Y.Z`.

## Next step toward silent auto-update
First choose a public binary distribution endpoint (or explicit user authentication). Once asset
downloads are available to the installed app, it can download to a temp folder and launch the
installer with `/VERYSILENT`; `ReleaseInfo.DownloadUrl` already exposes the selected asset URL.
