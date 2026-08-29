namespace GameScout.Core.Updating;

/// <summary>Metadata about a published release, normalized from the release host (GitHub).</summary>
/// <param name="TagName">The raw tag (e.g. "v0.2.0").</param>
/// <param name="Version">The parsed version, or null when the tag isn't a version.</param>
/// <param name="Name">The release title, if any.</param>
/// <param name="HtmlUrl">The human-facing release page.</param>
/// <param name="DownloadUrl">Direct download URL for an installer asset, if present.</param>
/// <param name="PublishedUtc">When the release was published, if known.</param>
public sealed record ReleaseInfo(
    string TagName,
    Version? Version,
    string? Name = null,
    string? HtmlUrl = null,
    string? DownloadUrl = null,
    DateTimeOffset? PublishedUtc = null);
