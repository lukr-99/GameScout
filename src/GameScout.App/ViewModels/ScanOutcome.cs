namespace GameScout.App.ViewModels;

/// <summary>The result a <see cref="ScannerViewModel"/> reports back after a scan.</summary>
/// <param name="StatusText">Short status line to show under the header.</param>
/// <param name="Errors">Per-source error messages, empty when everything succeeded.</param>
public readonly record struct ScanOutcome(string StatusText, IReadOnlyList<string> Errors);
