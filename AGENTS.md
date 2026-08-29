# AGENTS

Guidance for coding agents working on FreeGameScout.

## Orientation
- Read `HANDOFF.md` first, then `RULES.md` and `STRUCTURE.md`.
- This is a WPF (.NET 10) Windows tray app with a platform-neutral `Core`. Keep `Core` free of any
  WPF/WinForms/registry dependency.

## Verification baseline
Run before committing:
1. `dotnet build FreeGameScout.slnx -c Release`
2. `dotnet test FreeGameScout.slnx -c Release`

Both must be clean (0 warnings — warnings are errors) and green (all tests pass).

## Conventions
- Follow `RULES.md`: MVVM boundaries, semantic theme keys (light + dark), one class per file,
  nullable enabled, `sealed` by default, XML docs on public APIs.
- New giveaway sources implement `IGiveawaySource`, depend on `IHttpTextClient`, expose a pure
  `static Parse(string)` and add a fixture-backed test under `tests/.../Sources/`.
- Windows-only code stays in `FreeGameScout.App`.

## Documentation discipline
- Append meaningful work to `WORKLOG.md`.
- Keep `TODO.md` to actionable next slices.
- Update `INTENT.md` at session close with current intent + next steps.
