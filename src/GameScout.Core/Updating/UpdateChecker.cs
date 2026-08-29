namespace GameScout.Core.Updating;

/// <summary>Compares the running version against the latest release to detect an available update.</summary>
public sealed class UpdateChecker
{
    private readonly IReleaseSource _source;

    /// <summary>Initializes a new <see cref="UpdateChecker"/>.</summary>
    /// <param name="source">The release source to query.</param>
    public UpdateChecker(IReleaseSource source)
        => _source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>
    /// Returns the latest release when it is newer than <paramref name="current"/>; otherwise null.
    /// </summary>
    /// <param name="current">The running application version.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    public async Task<ReleaseInfo?> CheckForUpdateAsync(
        Version current, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(current);

        ReleaseInfo? latest = await _source.GetLatestAsync(cancellationToken).ConfigureAwait(false);
        if (latest?.Version is null)
            return null;

        return latest.Version > Normalize(current) ? latest : null;
    }

    // Compare on major.minor.build only; a 4-part assembly version's revision is ignored.
    private static Version Normalize(Version v)
        => new(v.Major, v.Minor, Math.Max(0, v.Build));
}
