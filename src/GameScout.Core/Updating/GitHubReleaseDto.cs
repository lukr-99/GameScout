using System.Text.Json.Serialization;

namespace GameScout.Core.Updating;

/// <summary>Subset of the GitHub "latest release" payload we consume.</summary>
internal sealed class GitHubReleaseDto
{
    [JsonPropertyName("tag_name")]
    public string? TagName { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; init; }

    [JsonPropertyName("published_at")]
    public DateTimeOffset? PublishedAt { get; init; }

    [JsonPropertyName("assets")]
    public IReadOnlyList<GitHubAssetDto>? Assets { get; init; }
}

internal sealed class GitHubAssetDto
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("browser_download_url")]
    public string? DownloadUrl { get; init; }
}
