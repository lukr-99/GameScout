using System.Text.Json;
using GameScout.Core.Abstractions;

namespace GameScout.Core.Updating;

/// <summary>Reads the latest GitHub release for a repository via the public REST API.</summary>
public sealed class GitHubReleaseSource : IReleaseSource
{
    private static readonly string[] InstallerExtensions = [".exe", ".msi", ".msix"];

    private readonly IHttpTextClient _http;
    private readonly string _endpoint;

    /// <summary>Initializes a new <see cref="GitHubReleaseSource"/>.</summary>
    /// <param name="http">Transport used to fetch the release (must send a User-Agent for GitHub).</param>
    /// <param name="owner">Repository owner (default "lukr-99").</param>
    /// <param name="repo">Repository name (default "GameScout").</param>
    public GitHubReleaseSource(IHttpTextClient http, string owner = "lukr-99", string repo = "GameScout")
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _endpoint = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
    }

    /// <inheritdoc/>
    public async Task<ReleaseInfo?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            string json = await _http.GetStringAsync(_endpoint, cancellationToken).ConfigureAwait(false);
            return Parse(json);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // No releases yet (404), rate-limited, or offline: treat as "no update info".
            return null;
        }
    }

    /// <summary>Parses a GitHub "latest release" payload into a <see cref="ReleaseInfo"/>. Exposed for testing.</summary>
    /// <param name="json">The raw JSON body.</param>
    /// <returns>The release info, or null when the tag is missing.</returns>
    public static ReleaseInfo? Parse(string json)
    {
        GitHubReleaseDto? dto = JsonSerializer.Deserialize<GitHubReleaseDto>(json);
        if (dto?.TagName is not { Length: > 0 } tag)
            return null;

        Version? version = ReleaseVersion.TryParse(tag, out Version parsed) ? parsed : null;
        return new ReleaseInfo(
            TagName: tag,
            Version: version,
            Name: dto.Name,
            HtmlUrl: dto.HtmlUrl,
            DownloadUrl: PickInstaller(dto.Assets),
            PublishedUtc: dto.PublishedAt);
    }

    private static string? PickInstaller(IReadOnlyList<GitHubAssetDto>? assets)
    {
        if (assets is null)
            return null;

        foreach (GitHubAssetDto asset in assets)
        {
            if (asset.Name is { } name && asset.DownloadUrl is { Length: > 0 } url &&
                InstallerExtensions.Any(ext => name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                return url;
        }

        return null;
    }
}
