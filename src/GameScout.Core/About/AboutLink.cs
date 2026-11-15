namespace GameScout.Core.About;

/// <summary>A labelled external link shown on the About page.</summary>
/// <param name="Label">Service name, such as "Instagram".</param>
/// <param name="Handle">Short text shown next to the label, such as "@lukas9krejci".</param>
/// <param name="Url">Absolute https address opened in the default browser.</param>
public sealed record AboutLink(string Label, string Handle, string Url);
