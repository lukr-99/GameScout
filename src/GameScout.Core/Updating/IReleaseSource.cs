namespace GameScout.Core.Updating;

/// <summary>Provides the latest published release, or null when none is available/reachable.</summary>
public interface IReleaseSource
{
    /// <summary>Fetches the latest release, or null on failure or when there are no releases.</summary>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    Task<ReleaseInfo?> GetLatestAsync(CancellationToken cancellationToken = default);
}
