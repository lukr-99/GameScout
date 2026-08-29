# RULES

Engineering rules for GameScout (adapted from the sibling `dotnetlib` conventions).

## Architecture
- Dependency direction: `App -> Core`. Never the reverse.
- `GameScout.Core` stays platform-neutral (`net10.0`, no WPF/WinForms). All Windows-specific
  code (UI, tray, registry) lives in `GameScout.App`.
- Giveaway sources depend on `IHttpTextClient`, never on a concrete `HttpClient`, so they stay
  unit-testable without the network.

## MVVM boundaries
- Code-behind is for view wiring only (InitializeComponent, DataContext, window show/hide).
- Domain and parsing logic belong in `Core`.
- View state + commands live in view-models using `ObservableObject` and `RelayCommand`.

## Sources
- An `IGiveawaySource` must not throw for expected "nothing found" cases — return an empty list.
- Parsing must be exposed as a pure `static Parse(string json)` method so it can be tested against a
  fixed sample with no network access.
- Network failures are handled by the aggregator (recorded as a per-source error), not by crashing.

## Theming
- Every UI surface uses **semantic resource keys** via `DynamicResource`, never hard-coded colors.
- Both **light and dark** are defined with the same keys, and the theme is switchable at runtime.

## Coding standards
- Nullable reference types enabled; warnings treated as errors.
- Prefer `sealed` classes; one class per file; file name == type name.
- Public APIs carry XML summaries.
- No static mutable state without a documented reason.

## Testing
- Every production layer in `Core` has test coverage.
- Tests are deterministic: no real time, network, or environment coupling (inject `TimeProvider`
  and `IHttpTextClient`; parse fixtures from `Samples/`).
- Test names follow `Method_Condition_ExpectedResult`.

## CI
- CI enforces: format check, restore, build, test. Do not suppress warnings to pass a build.

## Documentation
- Keep `README.md`, `STRUCTURE.md`, `HANDOFF.md` aligned with real behavior.
- Append delivered work to `WORKLOG.md`; keep `TODO.md` to actionable next slices; refresh
  `INTENT.md` at session close.
